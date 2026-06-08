using Planademic.Domain;

namespace Planademic.BLL.Services;

public class SchedulingService : ISchedulingService
{
    private readonly IStudentTaskService _taskService;
    private readonly IAvailabilityService _availabilityService;

    public SchedulingService(IStudentTaskService taskService, IAvailabilityService availabilityService)
    {
        _taskService = taskService;
        _availabilityService = availabilityService;
    }

    public async Task<List<ScheduledTask>> GenerateScheduleAsync(int studentId)
    {
        var tasks = await _taskService.GetByStudentIdAsync(studentId);
        var slots = await _availabilityService.GetSlotsAsync(studentId);
        // Filter to only pending tasks
        var pending = new List<StudentTask>();
        foreach (var task in tasks)
        {
            if (!task.IsCompleted)
                pending.Add(task);
        }

        if (pending.Count == 0 || slots.Count == 0)
            return new List<ScheduledTask>();

        // Sort tasks by priority score, highest first
        var prioritised = pending
            .OrderByDescending(t => CalculatePriority(t))
            .ThenBy(t => t.Id)
            .ToList();

        // Map each slot to its date in the current Monday–Sunday week
        // Using % 7 to convert Sunday=0 to 6, Monday=1 to 0, etc.
        var today = DateOnly.FromDateTime(DateTime.Today);
        int daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
        var weekStart = today.AddDays(-daysFromMonday);

        var upcomingSlots = new List<(AvailabilitySlot Slot, DateOnly Date)>();
        foreach (var slot in slots)
        {
            int offset = ((int)slot.DayOfWeek + 6) % 7;
            var date = weekStart.AddDays(offset);
            upcomingSlots.Add((slot, date));
        }
        upcomingSlots = upcomingSlots.OrderBy(x => x.Date).ThenBy(x => x.Slot.StartTime).ToList();

        // Build a weighted pool: complex tasks appear more times so they get more slots
        var pool = new List<StudentTask>();
        foreach (var task in prioritised)
        {
            int count = SlotsNeeded(task);
            for (int j = 0; j < count; j++)
                pool.Add(task);
        }

        // Fill every available slot, cycling through the pool so no slot is ever left empty
        var result = new List<ScheduledTask>();
        for (int i = 0; i < upcomingSlots.Count; i++)
        {
            var pair = upcomingSlots[i];
            var task = pool[i % pool.Count];
            result.Add(new ScheduledTask(
                task,
                pair.Slot.DayOfWeek,
                pair.Date,
                pair.Slot.StartTime,
                pair.Slot.EndTime));
        }

        return result;
    }

    // Priority score: tasks that are complex and due soon score highest
    public static double CalculatePriority(StudentTask task)
    {
        var daysRemaining = (task.Deadline.Date - DateTime.Today).Days;
        if (daysRemaining < 1)
            daysRemaining = 1;

        var urgency = 1.0 / daysRemaining;
        var score = (0.4 * task.Complexity) + (0.6 * urgency);
        return score;
    }

    // A complexity-10 task needs 5 slots, complexity-2 needs 1 slot, etc.
    private static int SlotsNeeded(StudentTask task)
    {
        int slots = (int)Math.Ceiling(task.Complexity / 2.0);
        if (slots < 1)
            slots = 1;
        return slots;
    }

}
