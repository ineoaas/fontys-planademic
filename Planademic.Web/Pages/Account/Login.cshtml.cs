using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;

namespace Planademic.Web.Pages.Account;

public class LoginModel : PageModel
{
    private readonly IUserService _userService;
    private const string HardcodedTeacherEmail = "justinleomaas@gmail.com";

    public LoginModel(IUserService userService)
    {
        _userService = userService;
    }

    [BindProperty]
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;

    [BindProperty]
    public string Role { get; set; } = "Student";

    public string? ErrorMessage { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        // Step 1: check data annotations ([Required], [EmailAddress])
        // If email is "notanemail" this will fail here and show the error
        if (!ModelState.IsValid)
        {
            ErrorMessage = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault();
            return Page();
        }

        // Step 2: teacher is hardcoded — only one email is allowed to log in as Teacher
        if (Role == "Teacher" &&
            !Email.Equals(HardcodedTeacherEmail, StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "Only authorized teachers can log in.";
            return Page();
        }

        // Step 3: look up the user in the database and verify password
        var user = await _userService.ValidateLoginAsync(Email, Password, Role);
        if (user == null)
        {
            ErrorMessage = "Invalid email or password.";
            return Page();
        }

        // Step 4: build the identity — these claims are stored inside the encrypted cookie
        // ClaimTypes.Role is what [Authorize(Roles = "Teacher")] checks against
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,  $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role,  user.Role),
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        // Step 5: write the encrypted cookie to the browser
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        // Step 6: redirect to the right dashboard based on role
        return user.Role == "Teacher"
            ? RedirectToPage("/Teacher/Dashboard")
            : RedirectToPage("/Dashboard");
    }
}
