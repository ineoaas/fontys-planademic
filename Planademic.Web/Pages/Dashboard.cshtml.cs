using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;
using Planademic.Domain;

namespace Planademic.Web.Pages
{
    [Authorize(Roles = "Student")]
    public class DashboardModel : PageModel
    {
        private readonly IStudentTaskService _taskService;
        private readonly IAvailabilityService _availabilityService;
        private readonly ISchedulingService _schedulingService;

        public int PendingCount { get; private set; }
        public int DueThisWeekCount { get; private set; }
        public int HighComplexityDueThisWeek { get; private set; }
        public int FreeHoursThisWeek { get; private set; }
        public int CompletedCount { get; private set; }
        public List<StudentTask> TopTasks { get; private set; } = [];
        public List<ScheduledTask> TodaySlots { get; private set; } = [];

        public DashboardModel(
            IStudentTaskService taskService,
            IAvailabilityService availabilityService,
            ISchedulingService schedulingService)
        {
            _taskService = taskService;
            _availabilityService = availabilityService;
            _schedulingService = schedulingService;
        }

        // On page load, fetch tasks, availability, and generate the schedule to populate dashboard metrics and today's slots.
        public async Task OnGetAsync()
        {
            var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var tasks    = await _taskService.GetByStudentIdAsync(studentId);
            var slots    = await _availabilityService.GetSlotsAsync(studentId);
            var schedule = await _schedulingService.GenerateScheduleAsync(studentId);

            var today   = DateTime.Today;
            var weekEnd = today.AddDays(7);

            var pending     = tasks.Where(t => !t.IsCompleted).ToList();
            var dueThisWeek = pending.Where(t => t.Deadline.Date <= weekEnd).ToList();

            PendingCount              = pending.Count;
            DueThisWeekCount          = dueThisWeek.Count;
            HighComplexityDueThisWeek = dueThisWeek.Count(t => t.Complexity >= 7);
            FreeHoursThisWeek         = slots.Count * 2;
            CompletedCount            = tasks.Count(t => t.IsCompleted);

            TopTasks = pending
                .OrderByDescending(SchedulingService.CalculatePriority)
                .ThenBy(t => t.Id)
                .Take(5)
                .ToList();

            TodaySlots = schedule
                .Where(s => s.Date == DateOnly.FromDateTime(today))
                .ToList();
        }
    }
}
