using Planademic.BLL.Services;
using UnitTesting.Fakes;

namespace UnitTesting;

public class TasksTests
{
    // TC-004-01: Teacher can add an assignment to a course
    [Fact]
    public async Task AddAssignment()
    {
        var fakeRepo = new FakeCourseTaskRepository();
        var service = new CourseTaskService(fakeRepo);
        var deadline = DateTime.UtcNow.AddDays(7);

        var (success, error) = await service.CreateCourseTaskAsync("Test Assignment", "Test Description", 5, deadline, 1);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(fakeRepo.AddedAssignment);
        Assert.Equal("Test Assignment", fakeRepo.AddedAssignment.Title);
    }

    // TC-004-02: Verify if an invalid complexity rating is rejected when adding an assignment
    [Fact]
    public async Task ReturnsErrorWhenComplexityIsInvalid()
    {
        var fakeRepo = new FakeCourseTaskRepository();
        var service = new CourseTaskService(fakeRepo);
        var deadline = DateTime.UtcNow.AddDays(7);

        var (success, error) = await service.CreateCourseTaskAsync("Test Assignment", "Test Description", 15, deadline, 1);

        Assert.False(success);
        Assert.NotNull(error);
    }

    // TC-004-03: Verify a Student can add a personal task
    [Fact]
    public async Task StudentCanAddPersonalTask()
    {
        var fakeRepo = new FakeStudentTaskRepository();
        var service = new StudentTaskService(fakeRepo);
        var deadline = DateTime.UtcNow.AddDays(7);

        var (success, error) = await service.CreateStudentTaskAsync("Study for Exam", 3, deadline, studentId: 1, assignmentId: null);

        Assert.True(success);
        Assert.Null(error);
        Assert.NotNull(fakeRepo.AddedStudentTask);
        Assert.Equal("Study for Exam", fakeRepo.AddedStudentTask.Title);
    }
    
    // Extra test cases for edge scenarios
    [Fact]
    public async Task ReturnsErrorWhenCourseIdIsInvalid()
    {
        var fakeRepo = new FakeCourseTaskRepository();
        var service = new CourseTaskService(fakeRepo);
        var deadline = DateTime.UtcNow.AddDays(7);

        var (success, error) = await service.CreateCourseTaskAsync("Test Assignment", "Test Description", 5, deadline, -1);

        Assert.False(success);
        Assert.NotNull(error);
    }

    [Fact]
    public async Task ReturnsErrorWhenDeadlineIsInThePast()
    {
        var fakeRepo = new FakeCourseTaskRepository();
        var service = new CourseTaskService(fakeRepo);
        var deadline = DateTime.UtcNow.AddDays(-1);

        var (success, error) = await service.CreateCourseTaskAsync("Test Assignment", "Test Description", 5, deadline, 1);

        Assert.False(success);
        Assert.NotNull(error);
    }
}