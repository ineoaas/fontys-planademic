using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public interface ICourseTaskRepository
{
    Task AddAsync(Assignment assignment);
    Task<List<Assignment>> GetByCourseIdAsync(int courseId);
    Task<List<Assignment>> GetByEnrolledStudentAsync(int studentId);
    Task<bool> DeleteAsync(int assignmentId);
}
