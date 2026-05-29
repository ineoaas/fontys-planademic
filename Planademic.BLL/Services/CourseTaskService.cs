using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace Planademic.BLL.Services;

public class CourseTaskService : ICourseTaskService
{
    private readonly ICourseTaskRepository _taskRepo;

    public CourseTaskService(ICourseTaskRepository taskRepo)
    {
        _taskRepo = taskRepo;
    }

// This method is the same as the StudentTaskService one.
    public async Task<(bool Success, string? Error)> CreateCourseTaskAsync(string title, string description, int complexity, DateTime deadline, int courseId)
    {
        if (string.IsNullOrWhiteSpace(title))
            return (false, "Title is required.");

        if (complexity < 1 || complexity > 10)
            return (false, "Complexity must be between 1 and 10.");

        if (deadline <= DateTime.UtcNow)
            return (false, "Deadline must be in the future.");

        var assignment = new Assignment
        {
            Title = title.Trim(),
            Description = description.Trim(),
            Complexity = complexity,
            Deadline = deadline,
            CourseId = courseId,
            CreatedAt = DateTime.UtcNow,
        };

        await _taskRepo.AddAsync(assignment);
        return (true, null);
    }

    public async Task<List<Assignment>> GetTasksByCourseAsync(int courseId)
    {
        return await _taskRepo.GetByCourseIdAsync(courseId);
    }

    public async Task<List<Assignment>> GetTasksByStudentEnrollmentsAsync(int studentId)
    {
        return await _taskRepo.GetByEnrolledStudentAsync(studentId);
    }

    public async Task<bool> DeleteAssignmentAsync(int assignmentId)
    {
        return await _taskRepo.DeleteAsync(assignmentId);
    }
}
