using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Office;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OfficesController : ControllerBase
    {
        private readonly IOfficeService _officeService;

        public OfficesController(IOfficeService officeService)
        {
            _officeService = officeService;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        [HttpGet]
        [HasPermission("Offices.View")]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null, [FromQuery] bool? isActive = null)
        {
            var result = await _officeService.GetPagedAsync(pageNumber, pageSize, search, isActive);
            return Ok(new { success = true, message = "Offices retrieved successfully.", data = result });
        }

        [HttpGet("{id}")]
        [HasPermission("Offices.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var office = await _officeService.GetByIdAsync(id);
            return office == null
                ? NotFound(new { success = false, message = "Office not found." })
                : Ok(new { success = true, message = "Office retrieved successfully.", data = office });
        }

        [HttpPost]
        [HasPermission("Offices.Create")]
        public async Task<IActionResult> Create([FromBody] OfficeCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var created = await _officeService.CreateAsync(dto, GetCurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, new { success = true, message = "Office created successfully.", data = created });
        }

        [HttpPut("{id}")]
        [HasPermission("Offices.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] OfficeUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            if (!await _officeService.UpdateAsync(id, dto, GetCurrentUserId()))
                return NotFound(new { success = false, message = "Office not found." });

            return Ok(new { success = true, message = "Office updated successfully." });
        }

        [HttpDelete("{id}")]
        [HasPermission("Offices.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _officeService.DeleteAsync(id, GetCurrentUserId()))
                return NotFound(new { success = false, message = "Office not found." });

            return NoContent();
        }
    }
}
