using Planademic.BLL.Services;
using Planademic.Domain;

namespace UnitTesting.Fakes;

public class FakeAvailabilityService : IAvailabilityService
{
    public List<AvailabilitySlot> SlotsToReturn = new();

    public Task<List<AvailabilitySlot>> GetSlotsAsync(int studentId)
    {
        return Task.FromResult(SlotsToReturn);
    }

    public Task SaveSlotsAsync(int studentId, List<AvailabilitySlot> slots)
    {
        SlotsToReturn = slots;
        return Task.CompletedTask;
    }

    public Task DeleteSlotAsync(int slotId)
    {
        for (int i = SlotsToReturn.Count - 1; i >= 0; i--)
        {
            if (SlotsToReturn[i].Id == slotId)
                SlotsToReturn.RemoveAt(i);
        }
        return Task.CompletedTask;
    }
}
