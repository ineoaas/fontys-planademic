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
    public class IndexModel : PageModel
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

    public async Task<IActionResult> OnPostDeleteAsync(int courseId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _courseService.DeleteCourseAsync(courseId, userId);
        return RedirectToPage();
    }
}
