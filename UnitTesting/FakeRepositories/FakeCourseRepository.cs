using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace UnitTesting.Fakes;

public class FakeCourseRepository : ICourseRepository
{
    public Course? AddedCourse = null;
    public Course? CourseToReturn = null;
    public bool IsEnrolled = false;
    public CourseEnrollment? AddedEnrollment = null;

    public Task AddAsync(Course course)
    {
        AddedCourse = course;
        return Task.CompletedTask;
    }

    public Task<Course?> GetByJoinCodeAsync(string joinCode)
    {
        return Task.FromResult(CourseToReturn);
    }

    public Task<List<Course>> GetByUserIdAsync(int userId)
    {
        return Task.FromResult(new List<Course>());
    }

    public Task AddEnrollmentAsync(CourseEnrollment enrollment)
    {
        AddedEnrollment = enrollment;
        return Task.CompletedTask;
    }

    public Task<bool> IsEnrolledAsync(int courseId, int studentId)
    {
        return Task.FromResult(IsEnrolled);
    }

    public Task<bool> DeleteAsync(int courseId, int teacherId)
    {
        return Task.FromResult(true);
    }

    public Task<List<User>> GetStudentsByCourseIdAsync(int courseId, int teacherId)
    {
        return Task.FromResult(new List<User>());
    }
}
