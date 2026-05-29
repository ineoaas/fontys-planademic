using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Planademic.BLL.Services;

namespace Planademic.Web.Pages.Availability
{
    [Authorize(Roles = "Student")]
    public class IndexModel : PageModel
    {
        private readonly IAvailabilityService _availabilityService;
        
        // HashSet used to check instantly if a slot is selected. Saves the need to loop through big list
        public HashSet<(int Day, int Slot)> SavedSlots { get; private set; } = [];

        public IndexModel(IAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        public async Task OnGetAsync()
        {
            var studentId = GetStudentId();
            var slots = await _availabilityService.GetSlotsAsync(studentId);
            SavedSlots = AvailabilityService.ToGridSet(slots);
        }

        public async Task<IActionResult> OnPostSaveAsync([FromBody] List<SlotDto> slots)
        {
            var studentId = GetStudentId();
            var selections = slots.Select(s => (s.Day, s.Slot)).ToList();
            var domainSlots = AvailabilityService.BuildSlots(studentId, selections);
            await _availabilityService.SaveSlotsAsync(studentId, domainSlots);
            return new OkResult();
        }

        private int GetStudentId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public record SlotDto(int Day, int Slot);
    }
}
