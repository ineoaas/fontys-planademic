using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;

namespace Planademic.Web.Pages.Schedule
{
    [Authorize(Roles = "Student")]
    public class IndexModel : PageModel
    {
        private readonly ISchedulingService _schedulingService;

        public List<ScheduledTask> Schedule { get; private set; } = [];
        public DateOnly WeekStart { get; private set; }
        public DateOnly WeekEnd { get; private set; }

        public IndexModel(ISchedulingService schedulingService)
        {
            _schedulingService = schedulingService;
        }

        public async Task OnGetAsync()
        {
            var studentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Schedule = await _schedulingService.GenerateScheduleAsync(studentId);

            var today = DateOnly.FromDateTime(DateTime.Today);
            int daysFromMonday = ((int)today.DayOfWeek + 6) % 7;
            WeekStart = today.AddDays(-daysFromMonday);
            WeekEnd = WeekStart.AddDays(6);
        }

        // Returns 0–4 intensity for the heatmap cell (day × time period).
        // timePeriod: 0 = morning (before 12:00), 1 = afternoon (12–18), 2 = evening (18+)
        public int HeatmapLevel(DayOfWeek day, int timePeriod)
            => Math.Min(Schedule.Count(s => s.Day == day && TimePeriod(s.StartTime) == timePeriod), 4);

        private static int TimePeriod(TimeOnly t) => t.Hour < 12 ? 0 : t.Hour < 18 ? 1 : 2;
    }
}
