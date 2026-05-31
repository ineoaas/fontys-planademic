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

        // Find the next date each slot occurs and sort them chronologically
        var today = DateOnly.FromDateTime(DateTime.Today);
        var upcomingSlots = new List<(AvailabilitySlot Slot, DateOnly Date)>();
        foreach (var slot in slots)
        {
            var date = NextOccurrence(today, slot.DayOfWeek);
            upcomingSlots.Add((slot, date));
        }
        upcomingSlots = upcomingSlots.OrderBy(x => x.Date).ThenBy(x => x.Slot.StartTime).ToList();

        // Assign tasks to slots in priority order
        var result = new List<ScheduledTask>();
        int taskIndex = 0;
        int slotsLeft = SlotsNeeded(prioritised[0]);

        foreach (var pair in upcomingSlots)
        {
            if (taskIndex >= prioritised.Count)
                break;

            var currentTask = prioritised[taskIndex];
            result.Add(new ScheduledTask(
                currentTask,
                pair.Slot.DayOfWeek,
                pair.Date,
                pair.Slot.StartTime,
                pair.Slot.EndTime));

            slotsLeft--;
            if (slotsLeft == 0)
            {
                taskIndex++;
                if (taskIndex < prioritised.Count)
                    slotsLeft = SlotsNeeded(prioritised[taskIndex]);
            }
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

    // Walk forward day by day until we land on the right day of the week
    private static DateOnly NextOccurrence(DateOnly from, DayOfWeek target)
    {
        var current = from;
        while (current.DayOfWeek != target)
            current = current.AddDays(1);
        return current;
    }
}
