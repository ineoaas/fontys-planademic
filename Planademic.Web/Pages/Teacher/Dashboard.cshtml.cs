using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Planademic.Web.Pages.Teacher
{
    [Authorize(Roles = "Teacher")]
    public class DashboardModel : PageModel
    {
        public void OnGet() { }
    }
}