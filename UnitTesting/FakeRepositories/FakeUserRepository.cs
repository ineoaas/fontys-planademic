using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace UnitTesting.Fakes;

public class FakeUserRepository : IUserRepository
{
    public User? UserToReturn = null;
    public bool EmailExists = false;
    public User? AddedUser = null;

    public Task<User?> GetByEmailAsync(string email)
    {
        return Task.FromResult(UserToReturn);
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        return Task.FromResult(EmailExists);
    }

    public Task AddAsync(User user)
    {
        AddedUser = user;
        return Task.CompletedTask;
    }
}
