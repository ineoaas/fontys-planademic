using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Planademic.Web.Pages
{
    [Authorize(Roles = "Student")]
    public class DashboardModel : PageModel
    {
        public void OnGet() { }
    }
}