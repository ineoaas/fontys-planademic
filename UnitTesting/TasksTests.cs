using Planademic.BLL.Services;
using UnitTesting.Fakes;

namespace UnitTesting;

public class TasksTests
{
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
}