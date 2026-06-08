using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace UnitTesting.Fakes;

public class FakeCourseTaskRepository : ICourseTaskRepository
{
    public Assignment? AddedAssignment = null;
    public List<Assignment> AssignmentsToReturn = new();
    public bool DeleteResult = true;

    public Task AddAsync(Assignment assignment)
    {
        AddedAssignment = assignment;
        return Task.CompletedTask;
    }

    public Task<List<Assignment>> GetByCourseIdAsync(int courseId)
    {
        return Task.FromResult(AssignmentsToReturn);
    }

    public Task<List<Assignment>> GetByEnrolledStudentAsync(int studentId)
    {
        return Task.FromResult(AssignmentsToReturn);
    }
    
    public Task<bool> DeleteAsync(int assignmentId)
    {
        return Task.FromResult(DeleteResult);
    }
}
