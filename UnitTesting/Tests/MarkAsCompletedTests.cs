using Planademic.BLL.Services;
using Planademic.Domain;
using UnitTesting.Fakes;

namespace UnitTesting;

public class MarkAsCompletedTests
{
    // TC-008-01: Verify a Student can mark a task as completed
    [Fact]
    public async Task StudentCanMarkTaskAsCompleted()
    {
        var fakeRepo = new FakeStudentTaskRepository();
        var service = new StudentTaskService(fakeRepo);

        fakeRepo.MarkCompleteResult = true;

        var result = await service.MarkCompleteAsync(taskId: 1, studentId: 1);

        Assert.True(result);
    }

    // TC-008-02: Verify a completed task no longer appears in newly generated schedules
    [Fact]
    public async Task CompletedTaskIsExcludedFromGeneratedSchedule()
    {
        var fakeTaskService = new FakeStudentTaskService();
        var fakeAvailabilityService = new FakeAvailabilityService();

        fakeTaskService.TasksToReturn = new List<StudentTask>
        {
            new StudentTask { Id = 1, Title = "Completed Task", Deadline = DateTime.Today.AddDays(3), Complexity = 2, IsCompleted = true },
            new StudentTask { Id = 2, Title = "Pending Task", Deadline = DateTime.Today.AddDays(7), Complexity = 2, IsCompleted = false }
        };
        fakeAvailabilityService.SlotsToReturn = new List<AvailabilitySlot>
        {
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0) }
        };

        var schedulingService = new SchedulingService(fakeTaskService, fakeAvailabilityService);
        var schedule = await schedulingService.GenerateScheduleAsync(studentId: 1);

        foreach (var entry in schedule)
        {
            Assert.NotEqual("Completed Task", entry.Task.Title);
        }
        Assert.Equal("Pending Task", schedule[0].Task.Title);
    }
}
