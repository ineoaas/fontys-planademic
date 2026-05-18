using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;
using Planademic.Domain;

namespace Planademic.Web.Pages.Courses;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ICourseService _courseService;

    public IndexModel(ICourseService courseService)
    {
        _courseService = courseService;
    }

    [BindProperty]
    public string JoinCode { get; set; } = string.Empty;

    [BindProperty]
    public string CourseName { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public List<Course> Courses { get; set; } = [];

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Courses = await _courseService.GetCoursesByUserIdAsync(userId);
    }

    public async Task<IActionResult> OnPostJoinAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, error) = await _courseService.JoinCourseAsync(JoinCode, userId);

        if (!success)
        {
            ErrorMessage = error;
            Courses = await _courseService.GetCoursesByUserIdAsync(userId);
            return Page();
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, error) = await _courseService.CreateCourseAsync(CourseName, userId);

        if (!success)
        {
            ErrorMessage = error;
            Courses = await _courseService.GetCoursesByUserIdAsync(userId);
            return Page();
        }

        return RedirectToPage();
    }
}
