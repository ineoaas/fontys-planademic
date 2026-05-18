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
}
