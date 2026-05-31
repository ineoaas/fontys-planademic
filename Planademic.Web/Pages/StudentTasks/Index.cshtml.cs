using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;
using Planademic.Domain;

namespace Planademic.Web.Pages.StudentTasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IStudentTaskService _taskService;
    private readonly ICourseTaskService _courseTaskService;

    public IndexModel(IStudentTaskService taskService, ICourseTaskService courseTaskService)
    {
        _taskService = taskService;
        _courseTaskService = courseTaskService;
    }

    [BindProperty]
    public int AssignmentId { get; set; }

    [BindProperty]
    public string Title { get; set; } = string.Empty;

    [BindProperty]
    public int Complexity { get; set; }

    [BindProperty]
    public DateTime Deadline { get; set; }

    public string? ErrorMessage { get; set; }
    public List<Assignment> AvailableAssignments { get; set; } = [];
    public List<StudentTask> MyTasks { get; set; } = [];

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        AvailableAssignments = await _courseTaskService.GetTasksByStudentEnrollmentsAsync(userId);
        MyTasks = await _taskService.GetByStudentIdAsync(userId);
    }

// This is when a student click assign to me button, it will fetch the assignment object.
// If the assignment exist, it will create a new student task with the assignment details and save it to database.
    public async Task<IActionResult> OnPostAssignAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var allAssignments = await _courseTaskService.GetTasksByStudentEnrollmentsAsync(userId);
        var assignment = allAssignments.FirstOrDefault(a => a.Id == AssignmentId);
        if (assignment == null)
        {
            AvailableAssignments = allAssignments;
            MyTasks = await _taskService.GetByStudentIdAsync(userId);
            ErrorMessage = "Assignment not found.";
            return Page();
        }

        var (success, error) = await _taskService.CreateStudentTaskAsync(
            assignment.Title,
            assignment.Complexity,
            assignment.Deadline,
            userId,
            assignment.Id);

        if (!success)
        {
            ErrorMessage = error;
            AvailableAssignments = await _courseTaskService.GetTasksByStudentEnrollmentsAsync(userId);
            MyTasks = await _taskService.GetByStudentIdAsync(userId);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCreatePersonalAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (string.IsNullOrWhiteSpace(Title) || Complexity < 1 || Complexity > 10 || Deadline == default)
        {
            ErrorMessage = "Please fill in all fields with valid values.";
            AvailableAssignments = await _courseTaskService.GetTasksByStudentEnrollmentsAsync(userId);
            MyTasks = await _taskService.GetByStudentIdAsync(userId);
            return Page();
        }

        var (success, error) = await _taskService.CreateStudentTaskAsync(
            Title, Complexity, Deadline, userId, assignmentId: null);

        if (!success)
        {
            ErrorMessage = error;
            AvailableAssignments = await _courseTaskService.GetTasksByStudentEnrollmentsAsync(userId);
            MyTasks = await _taskService.GetByStudentIdAsync(userId);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int taskId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _taskService.MarkCompleteAsync(taskId, userId);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int taskId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _taskService.DeleteTaskAsync(taskId, userId);
        return RedirectToPage();
    }
}
