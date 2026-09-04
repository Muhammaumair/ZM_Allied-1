using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Vehicle;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/vehicles")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _service;

        public VehiclesController(IVehicleService service)
        {
            _service = service;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
        [HasPermission("Vehicles.View")]
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
                message = "Vehicles retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("Vehicles.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var vehicle = await _service.GetByIdAsync(id);

            if (vehicle is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Vehicle not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Vehicle retrieved successfully.",
                data = vehicle
            });
        }

        [HttpPost]
        [HasPermission("Vehicles.Create")]
        public async Task<IActionResult> Create(VehicleCreateDto dto)
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
                    message = "Vehicle created successfully.",
                    data = created
                });
        }

        [HttpPut("{id}")]
        [HasPermission("Vehicles.Update")]
        public async Task<IActionResult> Update(int id, VehicleUpdateDto dto)
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
                    message = "Vehicle not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Vehicle updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [HasPermission("Vehicles.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Vehicle not found."
                });
            }

            return NoContent();
        }
    }
}