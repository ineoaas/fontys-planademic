using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;
using Planademic.Domain;

namespace Planademic.Web.Pages.Teacher;

[Authorize]
public class StudentsModel : PageModel
{
    private readonly ICourseService _courseService;

    public StudentsModel(ICourseService courseService)
    {
        _courseService = courseService;
    }

    public int SelectedCourseId { get; set; }
    public List<Course> Courses { get; set; } = [];
    public List<User> Students { get; set; } = [];

    public async Task OnGetAsync(int courseId)
    {
        // Get the teacher's user ID from the claims. This assumes that the user ID is stored as a claim of type NameIdentifier.
        var teacherId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        Courses = await _courseService.GetCoursesByUserIdAsync(teacherId);

        if (Courses.Count > 0)
        {
            bool courseFound = false;
            foreach (var course in Courses)
            {
                if (course.Id == courseId)
                {
                    courseFound = true;
                    break;
                }
            }
            SelectedCourseId = courseFound ? courseId : Courses[0].Id;
            Students = await _courseService.GetStudentsInCourseAsync(SelectedCourseId, teacherId);
        }
    }
}
