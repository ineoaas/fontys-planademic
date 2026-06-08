using Planademic.Domain;
using Planademic.DAL.Repositories;

namespace UnitTesting.Fakes;

public class FakeAvailabilityRepository : IAvailabilityRepository
{
    public List<AvailabilitySlot> SlotsToReturn = new List<AvailabilitySlot>();
    public int? DeletedSlotId = null;
    public (int studentId, List<AvailabilitySlot> slots)? SavedRange = null;

    public Task<List<AvailabilitySlot>> GetByStudentIdAsync(int studentId)
    {
        return Task.FromResult(SlotsToReturn);
    }

    public Task SaveRangeAsync(int studentId, List<AvailabilitySlot> slots)
    {
        SavedRange = (studentId, slots);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(int slotId)
    {
        DeletedSlotId = slotId;
        return Task.CompletedTask;
    }
}