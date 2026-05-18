using Planademic.Domain;

namespace Planademic.BLL.Services;

public interface ICourseService
{
    Task<(bool Success, string? Error)> CreateCourseAsync(string name, int teacherId);
    Task<(bool Success, string? Error)> JoinCourseAsync(string joinCode, int studentId);
    Task<List<Course>> GetCoursesByUserIdAsync(int userId);
}
