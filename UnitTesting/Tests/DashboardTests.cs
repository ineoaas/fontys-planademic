using Planademic.BLL.Services;
using Planademic.Domain;
using UnitTesting.Fakes;

namespace UnitTesting;

public class DashboardTests
{
    // TC-006-01: Verify the student dashboard aggregates tasks from all joined courses
    [Fact]
    public async Task DashboardAggregatesTasksFromJoinedCourses()
    {
        var fakeRepo = new FakeStudentTaskRepository();
        var service = new StudentTaskService(fakeRepo);

        var studentId = 1;
        fakeRepo.StudentTasksToReturn = new List<StudentTask>
        {
            new StudentTask { Id = 1, StudentId = studentId, AssignmentId = 10, Title = "Course A Task", Deadline = DateTime.Today.AddDays(3), IsPersonal = false },
            new StudentTask { Id = 2, StudentId = studentId, AssignmentId = 20, Title = "Course B Task", Deadline = DateTime.Today.AddDays(5), IsPersonal = false }
        };

        var result = await service.GetByStudentIdAsync(studentId);

        Assert.Equal(2, result.Count);
        Assert.Equal("Course A Task", result[0].Title);
        Assert.Equal("Course B Task", result[1].Title);
    }

    // TC-006-02: Verify personal tasks appear on the dashboard alongside course assignments
    [Fact]
    public async Task PersonalTasksAppearAlongsideCourseAssignments()
    {
        var fakeRepo = new FakeStudentTaskRepository();
        var service = new StudentTaskService(fakeRepo);

        var studentId = 1;
        fakeRepo.StudentTasksToReturn = new List<StudentTask>
        {
            new StudentTask { Id = 1, StudentId = studentId, AssignmentId = 10, Title = "Course Assignment", Deadline = DateTime.Today.AddDays(3), IsPersonal = false },
            new StudentTask { Id = 2, StudentId = studentId, AssignmentId = null, Title = "Personal Task", Deadline = DateTime.Today.AddDays(5), IsPersonal = true }
        };

        var result = await service.GetByStudentIdAsync(studentId);

        Assert.Equal(2, result.Count);
        Assert.False(result[0].IsPersonal);
        Assert.True(result[1].IsPersonal);
    }
}
