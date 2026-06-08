using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace UnitTesting.Fakes;

public class FakeStudentTaskRepository : IStudentTaskRepository
{
    public StudentTask? AddedStudentTask = null;
    public List<StudentTask> StudentTasksToReturn = new();
    public bool DeleteResult = true;
    public bool MarkCompleteResult = true;

    public Task AddAsync(StudentTask studentTask)
    {
        AddedStudentTask = studentTask;
        return Task.CompletedTask;
    }

    public Task<List<StudentTask>> GetByStudentIdAsync(int studentId)
    {
        return Task.FromResult(StudentTasksToReturn);
    }

    public Task<bool> MarkCompleteAsync(int taskId, int studentId)
    {
        return Task.FromResult(MarkCompleteResult);
    }

    public Task<bool> DeleteAsync(int taskId, int studentId)
    {
        return Task.FromResult(DeleteResult);
    }
}