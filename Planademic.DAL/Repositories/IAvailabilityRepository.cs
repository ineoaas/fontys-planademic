using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public interface IAvailabilityRepository
{
    Task<List<AvailabilitySlot>> GetByStudentIdAsync(int studentId);
    Task SaveRangeAsync(int studentId, List<AvailabilitySlot> slots);
    Task DeleteAsync(int slotId);
}
