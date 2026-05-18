using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace Planademic.BLL.Services;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<(bool Success, string? Error)> CreateCourseAsync(string name, int teacherId)
    {
        var joinCode = GenerateJoinCode();

        var course = new Course
        {
            Name = name,
            JoinCode = joinCode,
            TeacherId = teacherId,
            CreatedAt = DateTime.UtcNow,
        };

        await _courseRepository.AddAsync(course);
        return (true, null);
    }

    public async Task<(bool Success, string? Error)> JoinCourseAsync(string joinCode, int studentId)
    {
        var course = await _courseRepository.GetByJoinCodeAsync(joinCode.Trim().ToUpper());
        if (course == null)
            return (false, "Course not found. Please check the join code.");

        if (await _courseRepository.IsEnrolledAsync(course.Id, studentId))
            return (false, "You are already enrolled in this course.");

        var enrollment = new CourseEnrollment
        {
            CourseId = course.Id,
            StudentId = studentId,
            EnrolledAt = DateTime.UtcNow,
        };

        await _courseRepository.AddEnrollmentAsync(enrollment);
        return (true, null);
    }

    public async Task<List<Course>> GetCoursesByUserIdAsync(int userId)
    {
        return await _courseRepository.GetByUserIdAsync(userId);
    }

    private static string GenerateJoinCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 6).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}
