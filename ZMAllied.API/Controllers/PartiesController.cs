using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Party;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PartiesController : ControllerBase
    {
        private readonly IPartyService _partyService;

        public PartiesController(IPartyService partyService)
        {
            _partyService = partyService;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        /// <summary>
        /// Get paginated list of parties with optional search.
        /// </summary>
        [HttpGet]
        [HasPermission("Parties.View")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _partyService.GetPagedAsync(pageNumber, pageSize, search);
            return Ok(new { success = true, message = "Parties retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get a party by ID.
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission("Parties.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var party = await _partyService.GetByIdAsync(id);
            if (party == null)
                return NotFound(new { success = false, message = "Party not found." });

            return Ok(new { success = true, message = "Party retrieved successfully.", data = party });
        }

        /// <summary>
        /// Create a new party.
        /// </summary>
        [HttpPost]
        [HasPermission("Parties.Create")]
        public async Task<IActionResult> Create([FromBody] PartyCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var created = await _partyService.CreateAsync(dto, GetCurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, message = "Party created successfully.", data = created });
        }

        /// <summary>
        /// Update an existing party.
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission("Parties.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] PartyUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _partyService.UpdateAsync(id, dto, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Party not found." });

            return Ok(new { success = true, message = "Party updated successfully." });
        }

        /// <summary>
        /// Soft delete a party.
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission("Parties.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _partyService.DeleteAsync(id, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Party not found." });

            return NoContent();
        }
    }
}
