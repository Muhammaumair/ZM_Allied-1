using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Trip;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Enums;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/trips")]
    [Authorize]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _service;

        public TripsController(ITripService service)
        {
            _service = service;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
        [HasPermission("Trips.View")]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            TripStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search, status, dateFrom, dateTo);

            return Ok(new
            {
                success = true,
                message = "Trips retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("Trips.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _service.GetByIdAsync(id);

            if (trip is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Trip not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Trip retrieved successfully.",
                data = trip
            });
        }

        [HttpPost]
        [HasPermission("Trips.Create")]
        public async Task<IActionResult> Create(TripCreateDto dto)
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
                    message = "Trip created successfully.",
                    data = created
                });
        }

        [HttpPut("{id}")]
        [HasPermission("Trips.Update")]
        public async Task<IActionResult> Update(int id, TripUpdateDto dto)
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
                    message = "Trip not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Trip updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [HasPermission("Trips.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Trip not found."
                });
            }

            return NoContent();
        }
    }
}