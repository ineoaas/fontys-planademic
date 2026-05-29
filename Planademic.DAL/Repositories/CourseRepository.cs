using Microsoft.EntityFrameworkCore;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly PlanademicDbContext _context;

    public CourseRepository(PlanademicDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Course course)
    {
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();
    }

    public async Task<Course?> GetByJoinCodeAsync(string joinCode)
    {
        return await _context.Courses
            .FirstOrDefaultAsync(c => c.JoinCode == joinCode);
    }

// This method builds two seperate queries for a the two different roles.
// Combines the results using Union to return a single list of courses.
    public async Task<List<Course>> GetByUserIdAsync(int userId)
    {
        var teacherCourses = _context.Courses.Where(c => c.TeacherId == userId);

        var enrolledCourseIds = _context.Enrollments
            .Where(e => e.StudentId == userId)
            .Select(e => e.CourseId);

        var studentCourses = _context.Courses.Where(c => enrolledCourseIds.Contains(c.Id));

        return await teacherCourses.Union(studentCourses).ToListAsync();
    }

    public async Task AddEnrollmentAsync(CourseEnrollment enrollment)
    {
        _context.Enrollments.Add(enrollment);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> IsEnrolledAsync(int courseId, int studentId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);
    }

// first deletes student tasks, then assignments, then enrollments, then the course.
    public async Task<bool> DeleteAsync(int courseId, int teacherId)
    {
        var course = await _context.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId && c.TeacherId == teacherId);

        if (course == null)
            return false;

        var assignmentIds = await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .Select(a => a.Id)
            .ToListAsync();

        if (assignmentIds.Count > 0)
        {
            var studentTasks = await _context.Tasks
                .Where(t => t.AssignmentId != null && assignmentIds.Contains(t.AssignmentId!.Value))
                .ToListAsync();
            _context.Tasks.RemoveRange(studentTasks);
            await _context.SaveChangesAsync();
        }

        var assignments = await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .ToListAsync();
        _context.Assignments.RemoveRange(assignments);
        await _context.SaveChangesAsync();

        var enrollments = await _context.Enrollments
            .Where(e => e.CourseId == courseId)
            .ToListAsync();
        _context.Enrollments.RemoveRange(enrollments);
        await _context.SaveChangesAsync();

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();
        return true;
    }
}
