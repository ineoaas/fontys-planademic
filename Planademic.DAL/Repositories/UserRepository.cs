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

    // Get by email finds the user by email. first or default returns null if no one matches.
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

    // First stages the new row in memory, then sends the insert command to the server.
    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
}
