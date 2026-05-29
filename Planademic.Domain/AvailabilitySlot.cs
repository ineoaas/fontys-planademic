namespace Planademic.Domain;

public class AvailabilitySlot
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsRecurring { get; set; }
}
