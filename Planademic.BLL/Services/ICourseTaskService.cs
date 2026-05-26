using Planademic.Domain;

namespace Planademic.BLL.Services;

public interface ICourseTaskService
{
    Task<(bool Success, string? Error)> CreateCourseTaskAsync(string title, string description, int complexity, DateTime deadline, int courseId);
    Task<List<Assignment>> GetTasksByCourseAsync(int courseId);
    Task<List<Assignment>> GetTasksByStudentEnrollmentsAsync(int studentId);
    Task<bool> DeleteAssignmentAsync(int assignmentId);
}
