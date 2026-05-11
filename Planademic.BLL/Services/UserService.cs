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

// Using Async from martins workshop
    public async Task<User?> ValidateLoginAsync(string email, string password, string role)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;

        if (!user.Role.Equals(role, StringComparison.OrdinalIgnoreCase)) return null;

        if (user.PasswordHash != password) return null;

        return user;
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(
        string email, string password, string firstName, string lastName)
    {
        if (await _userRepository.EmailExistsAsync(email))
            return (false, "An account with this email already exists.");

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
        return (true, null);
    }
}
