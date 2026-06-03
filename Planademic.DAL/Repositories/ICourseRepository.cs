using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public interface ICourseRepository
{
    Task AddAsync(Course course);
    Task<Course?> GetByJoinCodeAsync(string joinCode);
    Task<List<Course>> GetByUserIdAsync(int userId);
    Task AddEnrollmentAsync(CourseEnrollment enrollment);
    Task<bool> IsEnrolledAsync(int courseId, int studentId);
    Task<bool> DeleteAsync(int courseId, int teacherId);
    Task<List<User>> GetStudentsByCourseIdAsync(int courseId, int teacherId);
}
