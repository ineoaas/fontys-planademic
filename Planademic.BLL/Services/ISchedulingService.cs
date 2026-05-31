using Planademic.Domain;

namespace Planademic.BLL.Services;

// A record because we want to easily create immutable objects that represent scheduled tasks.
public record ScheduledTask(
    StudentTask Task,
    DayOfWeek Day,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime
);

public interface ISchedulingService
{
    Task<List<ScheduledTask>> GenerateScheduleAsync(int studentId);
}
