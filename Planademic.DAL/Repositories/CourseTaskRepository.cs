using Microsoft.EntityFrameworkCore;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class CourseTaskRepository : ICourseTaskRepository
{
    private readonly PlanademicDbContext _context;

    public CourseTaskRepository(PlanademicDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Assignment assignment)
    {
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Assignment>> GetByCourseIdAsync(int courseId)
    {
        return await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<List<Assignment>> GetByEnrolledStudentAsync(int studentId)
    {
        var enrolledCourseIds = _context.Enrollments
            .Where(e => e.StudentId == studentId)
            .Select(e => e.CourseId);

        return await _context.Assignments
            .Where(a => enrolledCourseIds.Contains(a.CourseId))
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int assignmentId)
    {
        var assignment = await _context.Assignments
            .FirstOrDefaultAsync(a => a.Id == assignmentId);

        if (assignment == null)
            return false;

        // Remove student tasks referencing this assignment first
        var relatedTasks = await _context.Tasks
            .Where(t => t.AssignmentId == assignmentId)
            .ToListAsync();
        _context.Tasks.RemoveRange(relatedTasks);

        _context.Assignments.Remove(assignment);
        await _context.SaveChangesAsync();
        return true;
    }
}
