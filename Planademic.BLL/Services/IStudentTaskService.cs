using Planademic.Domain;

namespace Planademic.BLL.Services;

public interface IStudentTaskService
{
    Task<(bool Success, string? Error)> CreateStudentTaskAsync(string title, int complexity, DateTime deadline, int studentId, int? assignmentId);
    Task<List<StudentTask>> GetByStudentIdAsync(int studentId);
    Task<bool> MarkCompleteAsync(int taskId, int studentId);
    Task<bool> DeleteTaskAsync(int taskId, int studentId);
}
