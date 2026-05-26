using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace Planademic.BLL.Services;

public class StudentTaskService : IStudentTaskService
{
    private readonly IStudentTaskRepository _taskRepo;

    public StudentTaskService(IStudentTaskRepository taskRepo)
    {
        _taskRepo = taskRepo;
    }

    public async Task<(bool Success, string? Error)> CreateStudentTaskAsync(string title, int complexity, DateTime deadline, int studentId, int? assignmentId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (false, "Title is required.");

        if (complexity < 1 || complexity > 10)
            return (false, "Complexity must be between 1 and 10.");

        if (deadline <= DateTime.UtcNow)
            return (false, "Deadline must be in the future.");

        var task = new StudentTask
        {
            Title = title.Trim(),
            Complexity = complexity,
            Deadline = deadline,
            StudentId = studentId,
            AssignmentId = assignmentId,
            IsPersonal = assignmentId == null,
            IsCompleted = false,
            PriorityScore = null,
            CreatedAt = DateTime.UtcNow,
        };

        await _taskRepo.AddAsync(task);
        return (true, null);
    }

    public async Task<List<StudentTask>> GetByStudentIdAsync(int studentId)
    {
        return await _taskRepo.GetByStudentIdAsync(studentId);
    }

    public async Task<bool> MarkCompleteAsync(int taskId, int studentId)
    {
        return await _taskRepo.MarkCompleteAsync(taskId, studentId);
    }

    public async Task<bool> DeleteTaskAsync(int taskId, int studentId)
    {
        return await _taskRepo.DeleteAsync(taskId, studentId);
    }
}
