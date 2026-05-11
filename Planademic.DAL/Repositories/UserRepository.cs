using Microsoft.EntityFrameworkCore;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PlanademicDbContext _context;

    public UserRepository(PlanademicDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
