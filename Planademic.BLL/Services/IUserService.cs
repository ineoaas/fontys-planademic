using Planademic.Domain;

namespace Planademic.BLL.Services;

public interface IUserService
{
    Task<User?> ValidateLoginAsync(string email, string password, string role);
    Task<(bool Success, string? Error, User? User)> RegisterAsync(string email, string password, string firstName, string lastName);
}

// This uses async, learnt from workshop with martin