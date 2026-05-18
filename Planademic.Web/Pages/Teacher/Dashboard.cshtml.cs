using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;
using Planademic.Domain;

namespace Planademic.Web.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class DashboardModel : PageModel
{
    private readonly ICourseService _courseService;

    public DashboardModel(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public List<Course> Courses { get; set; } = [];

    public async Task OnGetAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Courses = await _courseService.GetCoursesByUserIdAsync(userId);
    }
}
