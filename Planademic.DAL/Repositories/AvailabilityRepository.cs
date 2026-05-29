using Microsoft.EntityFrameworkCore;
using Planademic.Domain;

namespace Planademic.DAL.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly PlanademicDbContext _context;

    public AvailabilityRepository(PlanademicDbContext context)
    {
        _context = context;
    }

    public async Task<List<AvailabilitySlot>> GetByStudentIdAsync(int studentId)
    {
        return await _context.AvailabilitySlots
            .Where(a => a.StudentId == studentId)
            .ToListAsync();
    }

// Delete all existing slots for this student
// Insert all the new ones
    public async Task SaveRangeAsync(int studentId, List<AvailabilitySlot> slots)
    {
        var existing = await _context.AvailabilitySlots
            .Where(a => a.StudentId == studentId)
            .ToListAsync();

        _context.AvailabilitySlots.RemoveRange(existing);
        _context.AvailabilitySlots.AddRange(slots);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int slotId)
    {
        var slot = await _context.AvailabilitySlots.FindAsync(slotId);
        if (slot is not null)
        {
            _context.AvailabilitySlots.Remove(slot);
            await _context.SaveChangesAsync();
        }
    }
}
