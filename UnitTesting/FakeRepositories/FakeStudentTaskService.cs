using Planademic.BLL.Services;
using Planademic.Domain;

namespace UnitTesting.Fakes;

public class FakeStudentTaskService : IStudentTaskService
{
    public List<StudentTask> TasksToReturn = new();
    public bool MarkCompleteResult = true;
    public bool DeleteResult = true;

    public Task<(bool Success, string? Error)> CreateStudentTaskAsync(string title, int complexity, DateTime deadline, int studentId, int? assignmentId)
    {
        var task = new StudentTask
        {
            Title = title,
            Complexity = complexity,
            Deadline = deadline,
            StudentId = studentId,
            AssignmentId = assignmentId,
            IsPersonal = assignmentId == null
        };
        TasksToReturn.Add(task);
        return Task.FromResult<(bool, string?)>((true, null));
    }

    public Task<List<StudentTask>> GetByStudentIdAsync(int studentId)
    {
        return Task.FromResult(TasksToReturn);
    }

    public Task<bool> MarkCompleteAsync(int taskId, int studentId)
    {
        foreach (var task in TasksToReturn)
        {
            if (task.Id == taskId)
                task.IsCompleted = true;
        }
        return Task.FromResult(MarkCompleteResult);
    }

    public Task<bool> DeleteTaskAsync(int taskId, int studentId)
    {
        return Task.FromResult(DeleteResult);
    }
}
