using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.VehicleMaintenance;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehicleMaintenanceController : ControllerBase
    {
        private readonly IVehicleMaintenanceService _maintenanceService;

        public VehicleMaintenanceController(IVehicleMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        /// <summary>
        /// Get paginated list of vehicle maintenance records with optional search.
        /// </summary>
        [HttpGet]
        [HasPermission("VehicleMaintenance.View")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _maintenanceService.GetPagedAsync(pageNumber, pageSize, search);
            return Ok(new { success = true, message = "Vehicle maintenance records retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get a vehicle maintenance record by ID.
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission("VehicleMaintenance.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _maintenanceService.GetByIdAsync(id);
            if (item == null)
                return NotFound(new { success = false, message = "Vehicle maintenance record not found." });

            return Ok(new { success = true, message = "Vehicle maintenance record retrieved successfully.", data = item });
        }

        /// <summary>
        /// Create a new vehicle maintenance record.
        /// </summary>
        [HttpPost]
        [HasPermission("VehicleMaintenance.Create")]
        public async Task<IActionResult> Create([FromBody] VehicleMaintenanceCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var created = await _maintenanceService.CreateAsync(dto, GetCurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, message = "Vehicle maintenance record created successfully.", data = created });
        }

        /// <summary>
        /// Update an existing vehicle maintenance record.
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission("VehicleMaintenance.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] VehicleMaintenanceUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _maintenanceService.UpdateAsync(id, dto, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Vehicle maintenance record not found." });

            return Ok(new { success = true, message = "Vehicle maintenance record updated successfully." });
        }

        /// <summary>
        /// Soft delete a vehicle maintenance record.
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission("VehicleMaintenance.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _maintenanceService.DeleteAsync(id, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Vehicle maintenance record not found." });

            return NoContent();
        }
    }
}
