using Planademic.BLL.Services;
using Planademic.Domain;
using UnitTesting.Fakes;

namespace UnitTesting;

public class AvailabilityTests
{
    // TC-005-01: Verify a Student can input and save their weekly availability
    [Fact]
    public async Task CanSaveWeeklyAvailability()
    {
        var fakeRepo = new FakeAvailabilityRepository();
        var service = new AvailabilityService(fakeRepo);

        var slotsToSave = new List<AvailabilitySlot>
        {
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(9, 0), EndTime = new TimeOnly(11, 0) },
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeOnly(14, 0), EndTime = new TimeOnly(16, 0) }
        };

        await service.SaveSlotsAsync(1, slotsToSave);

        Assert.NotNull(fakeRepo.SavedRange);
        Assert.Equal(1, fakeRepo.SavedRange.Value.studentId);
        Assert.Equal(slotsToSave, fakeRepo.SavedRange.Value.slots);
    }

    // TC-005-02: Verify updated availability overwrites the previous availability
    [Fact]
    public async Task UpdatedAvailabilityOverwritesPrevious()
    {
        var fakeRepo = new FakeAvailabilityRepository();
        var service = new AvailabilityService(fakeRepo);

        var initialSlots = new List<AvailabilitySlot>
        {
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Tuesday, StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(12, 0) }
        };
        await service.SaveSlotsAsync(1, initialSlots);

        var updatedSlots = new List<AvailabilitySlot>
        {
            new AvailabilitySlot { DayOfWeek = DayOfWeek.Thursday, StartTime = new TimeOnly(13, 0), EndTime = new TimeOnly(15, 0) }
        };
        await service.SaveSlotsAsync(1, updatedSlots);

        Assert.NotNull(fakeRepo.SavedRange);
        Assert.Equal(1, fakeRepo.SavedRange.Value.studentId);
        Assert.Equal(updatedSlots, fakeRepo.SavedRange.Value.slots);
    }
}