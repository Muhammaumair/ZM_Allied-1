using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.VehicleType;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/vehicle-types")]
    [Authorize]
    public class VehicleTypesController : ControllerBase
    {
        private readonly IVehicleTypeService _service;

        public VehicleTypesController(IVehicleTypeService service)
        {
            _service = service;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
        [HasPermission("VehicleTypes.View")]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            bool? isActive = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search, isActive);

            return Ok(new
            {
                success = true,
                message = "Vehicle types retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("VehicleTypes.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var vehicleType = await _service.GetByIdAsync(id);

            if (vehicleType is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Vehicle type not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Vehicle type retrieved successfully.",
                data = vehicleType
            });
        }

        [HttpPost]
        [HasPermission("VehicleTypes.Create")]
        public async Task<IActionResult> Create(VehicleTypeCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var userId = GetUserId();
            var created = await _service.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new
                {
                    success = true,
                    message = "Vehicle type created successfully.",
                    data = created
                });
        }

        [HttpPut("{id}")]
        [HasPermission("VehicleTypes.Update")]
        public async Task<IActionResult> Update(int id, VehicleTypeUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed.",
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var userId = GetUserId();
            var updated = await _service.UpdateAsync(id, dto, userId);

            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Vehicle type not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Vehicle type updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [HasPermission("VehicleTypes.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Vehicle type not found."
                });
            }

            return NoContent();
        }
    }
}