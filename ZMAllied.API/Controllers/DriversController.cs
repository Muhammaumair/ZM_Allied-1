using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Driver;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/drivers")]
    [Authorize]
    public class DriversController : ControllerBase
    {
        private readonly IDriverService _service;

        public DriversController(IDriverService service)
        {
            _service = service;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
        [HasPermission("Drivers.View")]
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
                message = "Drivers retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("Drivers.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var driver = await _service.GetByIdAsync(id);

            if (driver is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Driver not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Driver retrieved successfully.",
                data = driver
            });
        }

        [HttpPost]
        [HasPermission("Drivers.Create")]
        public async Task<IActionResult> Create(DriverCreateDto dto)
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
                    message = "Driver created successfully.",
                    data = created
                });
        }

        [HttpPut("{id}")]
        [HasPermission("Drivers.Update")]
        public async Task<IActionResult> Update(int id, DriverUpdateDto dto)
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
                    message = "Driver not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Driver updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [HasPermission("Drivers.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Driver not found."
                });
            }

            return NoContent();
        }
    }
}