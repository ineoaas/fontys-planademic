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

    // Grid day 0 = Monday, but DayOfWeek.Monday = 1 (Sunday = 0).
    // Convert: gridDay → DayOfWeek via (gridDay + 1) % 7
    // DayOfWeek → gridDay via ((int)DayOfWeek + 6) % 7
    public static List<AvailabilitySlot> BuildSlots(int studentId, List<(int Day, int Slot)> selections)
    {
        var result = new List<AvailabilitySlot>();

        foreach (var s in selections)
        {
            if (s.Slot < 0 || s.Slot >= SlotTimes.Length)
                continue;

            var slot = new AvailabilitySlot
            {
                StudentId   = studentId,
                DayOfWeek   = (DayOfWeek)((s.Day + 1) % 7),
                StartTime   = SlotTimes[s.Slot].Start,
                EndTime     = SlotTimes[s.Slot].End,
                IsRecurring = true,
            };

            result.Add(slot);
        }

        return result;
    }

    public static HashSet<(int Day, int Slot)> ToGridSet(List<AvailabilitySlot> slots)
    {
        var result = new HashSet<(int, int)>();
        foreach (var s in slots)
        {
            // Convert DayOfWeek to grid day index
            var idx = Array.FindIndex(SlotTimes, t => t.Start == s.StartTime);
            if (idx >= 0)
                result.Add((((int)s.DayOfWeek + 6) % 7, idx));
        }
        return result;
    }
}
