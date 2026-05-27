using Planademic.DAL.Repositories;
using Planademic.Domain;

namespace Planademic.BLL.Services;

public class AvailabilityService : IAvailabilityService
{
    // Maps slot index (0–5) to its start/end times matching the UI grid
    private static readonly (TimeOnly Start, TimeOnly End)[] SlotTimes =
    [
        (new TimeOnly(8,  0), new TimeOnly(10, 0)),
        (new TimeOnly(10, 0), new TimeOnly(12, 0)),
        (new TimeOnly(12, 0), new TimeOnly(14, 0)),
        (new TimeOnly(14, 0), new TimeOnly(16, 0)),
        (new TimeOnly(16, 0), new TimeOnly(18, 0)),
        (new TimeOnly(18, 0), new TimeOnly(20, 0)),
    ];

    private readonly IAvailabilityRepository _repo;

    public AvailabilityService(IAvailabilityRepository repo)
    {
        _repo = repo;
    }

    public Task<List<AvailabilitySlot>> GetSlotsAsync(int studentId)
        => _repo.GetByStudentIdAsync(studentId);

    public Task SaveSlotsAsync(int studentId, List<AvailabilitySlot> slots)
        => _repo.SaveRangeAsync(studentId, slots);

    public Task DeleteSlotAsync(int slotId)
        => _repo.DeleteAsync(slotId);

    // Helper used by the page model to build slots from the grid selection
    public static List<AvailabilitySlot> BuildSlots(int studentId, List<(int Day, int Slot)> selections)
    {
        return selections
            .Where(s => s.Slot >= 0 && s.Slot < SlotTimes.Length)
            .Select(s => new AvailabilitySlot
            {
                StudentId  = studentId,
                DayOfWeek  = (DayOfWeek)s.Day,
                StartTime  = SlotTimes[s.Slot].Start,
                EndTime    = SlotTimes[s.Slot].End,
                IsRecurring = true,
            })
            .ToList();
    }

    // Helper to convert stored slots back to (day, slotIndex) pairs for the grid
    public static HashSet<(int Day, int Slot)> ToGridSet(List<AvailabilitySlot> slots)
    {
        var result = new HashSet<(int, int)>();
        foreach (var s in slots)
        {
            var idx = Array.FindIndex(SlotTimes, t => t.Start == s.StartTime);
            if (idx >= 0)
                result.Add(((int)s.DayOfWeek, idx));
        }
        return result;
    }
}
