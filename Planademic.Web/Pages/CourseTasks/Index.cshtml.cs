using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;
using Planademic.Domain;

namespace Planademic.Web.Pages.CourseTasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ICourseTaskService _taskService;
    private readonly ICourseService _courseService;

    public IndexModel(ICourseTaskService taskService, ICourseService courseService)
    {
        _taskService = taskService;
        _courseService = courseService;
    }

    [BindProperty(SupportsGet = true)]
    public int SelectedCourseId { get; set; }

    [BindProperty]
    public int CourseId { get; set; }

    [BindProperty]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    public string Description { get; set; } = string.Empty;

    [BindProperty]
    public int Complexity { get; set; }

    [BindProperty]
    public DateTime Deadline { get; set; }

    public string? ErrorMessage { get; set; }
    public List<Course> Courses { get; set; } = [];
    public List<Assignment> Assignments { get; set; } = [];

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Courses = await _courseService.GetCoursesByUserIdAsync(userId);

        if (Courses.Count > 0)
        {
            var courseId = Courses.Any(c => c.Id == SelectedCourseId)
                ? SelectedCourseId
                : Courses[0].Id;

            SelectedCourseId = courseId;
            Assignments = await _taskService.GetTasksByCourseAsync(courseId);
        }
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, error) = await _taskService.CreateCourseTaskAsync(Title, Description, Complexity, Deadline, CourseId);

        if (!success)
        {
            ErrorMessage = error;
            Courses = await _courseService.GetCoursesByUserIdAsync(userId);
            Assignments = await _taskService.GetTasksByCourseAsync(CourseId);
            SelectedCourseId = CourseId;
            return Page();
        }

        return RedirectToPage(new { SelectedCourseId = CourseId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int assignmentId, int selectedCourseId)
    {
        await _taskService.DeleteAssignmentAsync(assignmentId);
        return RedirectToPage(new { SelectedCourseId = selectedCourseId });
    }
}
