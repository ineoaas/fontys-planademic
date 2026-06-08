using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace Planademic.BLL.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

// checks if user exists, if the roles match, and if the password is correct.
    public async Task<User?> ValidateLoginAsync(string email, string password, string role)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;

        if (!user.Role.Equals(role, StringComparison.OrdinalIgnoreCase)) return null;

        if (user.PasswordHash != password) return null;

        return user;
    }

// checks if email is already used, if not creates a new user and saves it to the database.
    public async Task<(bool Success, string? Error, User? User)> RegisterAsync(
        string email, string password, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(email))
            return (false, "All fields are required.", null);

        if (string.IsNullOrWhiteSpace(password))
            return (false, "All fields are required.", null);

        if (string.IsNullOrWhiteSpace(firstName))
            return (false, "All fields are required.", null);

        if (string.IsNullOrWhiteSpace(lastName))
            return (false, "All fields are required.", null);

        if (await _userRepository.EmailExistsAsync(email))
            return (false, "An account with this email already exists.", null);

        var user = new User
        {
            Email = email,
            PasswordHash = password,
            Role = "Student",
            FirstName = firstName,
            LastName = lastName,
            CreatedAt = DateTime.UtcNow,
        };

        await _userRepository.AddAsync(user);
        return (true, null, user);
    }
}
