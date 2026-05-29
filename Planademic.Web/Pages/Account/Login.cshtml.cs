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
        if (!ModelState.IsValid)
        {
            ErrorMessage = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault();
            return Page();
        }

// A user must match the harcoded teacher email to loh in as a teacher.
        if (Role == "Teacher" &&
            !Email.Equals(HardcodedTeacherEmail, StringComparison.OrdinalIgnoreCase))
        {
            ErrorMessage = "Only authorized teachers can log in.";
            return Page();
        }

        var user = await _userService.ValidateLoginAsync(Email, Password, Role);
        if (user == null)
        {
            ErrorMessage = "Invalid email or password.";
            return Page();
        }
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name,  $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role,  user.Role),
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return user.Role == "Teacher"
            ? RedirectToPage("/Teacher/Dashboard")
            : RedirectToPage("/Dashboard");
    }
}

// A claim is information about the user that is stored in the cookie.