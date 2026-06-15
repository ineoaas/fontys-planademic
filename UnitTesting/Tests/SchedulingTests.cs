using Planademic.BLL.Services;
using Planademic.Domain;
using UnitTesting.Fakes;

namespace UnitTesting;

public class SchedulingTests
{
    // TC-007-01: Verify the algorithm maps tasks into available time slots
    [Fact]
    public async Task ScheduleMapsTasksToAvailableSlots()
    {
        var fakeTaskService = new FakeStudentTaskService();
        var fakeAvailabilityService = new FakeAvailabilityService();

        fakeTaskService.TasksToReturn = new List<StudentTask>
        {
            new StudentTask { Id = 1, Title = "Task 1", Deadline = DateTime.Today.AddDays(5), Complexity = 2, IsCompleted = false }
        };
        fakeAvailabilityService.SlotsToReturn = new List<AvailabilitySlot>
        {
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0) },
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0) }
        };

        var service = new SchedulingService(fakeTaskService, fakeAvailabilityService);
        var result = await service.GenerateScheduleAsync(studentId: 1);

        Assert.NotEmpty(result);
        Assert.Equal("Task 1", result[0].Task.Title);
    }

    // TC-007-02: Verify tasks with higher priority are scheduled into the earliest available slots
    [Fact]
    public async Task HigherPriorityTaskScheduledIntoEarliestSlot()
    {
        var fakeTaskService = new FakeStudentTaskService();
        var fakeAvailabilityService = new FakeAvailabilityService();

        fakeTaskService.TasksToReturn = new List<StudentTask>
        {
            new StudentTask { Id = 1, Title = "Urgent Task", Deadline = DateTime.Today.AddDays(2), Complexity = 2, IsCompleted = false },
            new StudentTask { Id = 2, Title = "Less Urgent Task", Deadline = DateTime.Today.AddDays(10), Complexity = 2, IsCompleted = false }
        };
        fakeAvailabilityService.SlotsToReturn = new List<AvailabilitySlot>
        {
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0) },
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(11, 0) }
        };

        var service = new SchedulingService(fakeTaskService, fakeAvailabilityService);
        var result = await service.GenerateScheduleAsync(studentId: 1);

        Assert.Equal(2, result.Count);
        Assert.Equal("Urgent Task", result[0].Task.Title);
        Assert.Equal("Less Urgent Task", result[1].Task.Title);
    }

    // TC-007-03: Verify no tasks are scheduled outside of the student's defined availability
    [Fact]
    public async Task AllScheduledTasksFallWithinDefinedAvailability()
    {
        var fakeTaskService = new FakeStudentTaskService();
        var fakeAvailabilityService = new FakeAvailabilityService();

        fakeTaskService.TasksToReturn = new List<StudentTask>
        {
            new StudentTask { Id = 1, Title = "Task 1", Deadline = DateTime.Today.AddDays(5), Complexity = 2, IsCompleted = false }
        };
        var definedSlots = new List<AvailabilitySlot>
        {
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(10, 0) },
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0) }
        };
        fakeAvailabilityService.SlotsToReturn = definedSlots;

        var service = new SchedulingService(fakeTaskService, fakeAvailabilityService);
        var result = await service.GenerateScheduleAsync(studentId: 1);

        foreach (var scheduled in result)
        {
            bool matchesSlot = false;
            foreach (var slot in definedSlots)
            {
                if (slot.DayOfWeek == scheduled.Day &&
                    slot.StartTime == scheduled.StartTime &&
                    slot.EndTime == scheduled.EndTime)
                {
                    matchesSlot = true;
                    break;
                }
            }
            Assert.True(matchesSlot);
        }
    }
}
