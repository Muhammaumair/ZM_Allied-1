using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.DDR;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/ddr")]
    [Authorize]
    public class DDRController : ControllerBase
    {
        private readonly IDDRService _ddrService;

        public DDRController(IDDRService ddrService)
        {
            _ddrService = ddrService;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
        [HasPermission("DDR.View")]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            int? companyId = null,
            int? vehicleId = null,
            int? biltyId = null,
            int? brokerId = null,
            int? partyId = null)
        {
            var result = await _ddrService.GetPagedAsync(
                pageNumber,
                pageSize,
                search,
                dateFrom,
                dateTo,
                companyId,
                vehicleId,
                biltyId,
                brokerId,
                partyId);

            return Ok(new
            {
                success = true,
                message = "DDRs retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("DDR.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var ddr = await _ddrService.GetByIdAsync(id);

            if (ddr is null)
            {
                return NotFound(new { success = false, message = "DDR not found." });
            }

            return Ok(new
            {
                success = true,
                message = "DDR retrieved successfully.",
                data = ddr
            });
        }

        [HttpPost]
        [HasPermission("DDR.Create")]
        public async Task<IActionResult> Create(DDRCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
            }

            var userId = GetUserId();
            var created = await _ddrService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new { success = true, message = "DDR created successfully.", data = created });
        }

        [HttpPut("{id}")]
        [HasPermission("DDR.Update")]
        public async Task<IActionResult> Update(int id, DDRUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
            }

            var userId = GetUserId();
            var updated = await _ddrService.UpdateAsync(id, dto, userId);

            if (!updated)
            {
                return NotFound(new { success = false, message = "DDR not found." });
            }

            return Ok(new { success = true, message = "DDR updated successfully." });
        }

        [HttpDelete("{id}")]
        [HasPermission("DDR.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _ddrService.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new { success = false, message = "DDR not found." });
            }

            return NoContent();
        }
    }
}
