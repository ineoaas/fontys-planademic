using Microsoft.EntityFrameworkCore;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class StudentTaskRepository : IStudentTaskRepository
{
    private readonly PlanademicDbContext _context;

    public StudentTaskRepository(PlanademicDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(StudentTask task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
    }

    public async Task<List<StudentTask>> GetByStudentIdAsync(int studentId)
    {
        return await _context.Tasks
            .Where(t => t.StudentId == studentId)
            .ToListAsync();
    }

    public async Task<bool> MarkCompleteAsync(int taskId, int studentId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.StudentId == studentId);

        if (task == null)
            return false;

        task.IsCompleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int taskId, int studentId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.StudentId == studentId);

        if (task == null)
            return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}
