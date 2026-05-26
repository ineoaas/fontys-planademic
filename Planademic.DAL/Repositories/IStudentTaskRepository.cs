using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public interface IStudentTaskRepository
{
    Task AddAsync(StudentTask task);
    Task<List<StudentTask>> GetByStudentIdAsync(int studentId);
    Task<bool> MarkCompleteAsync(int taskId, int studentId);
    Task<bool> DeleteAsync(int taskId, int studentId);
}
