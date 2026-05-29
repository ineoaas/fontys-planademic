using Planademic.Domain;

namespace Planademic.BLL.Services;

public interface IAvailabilityService
{
    Task<List<AvailabilitySlot>> GetSlotsAsync(int studentId);
    Task SaveSlotsAsync(int studentId, List<AvailabilitySlot> slots);
    Task DeleteSlotAsync(int slotId);
}
