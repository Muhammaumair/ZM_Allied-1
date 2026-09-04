using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Bilty;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Enums;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/bilties")]
    [Authorize]
    public class BiltiesController : ControllerBase
    {
        private readonly IBiltyService _service;

        public BiltiesController(IBiltyService service)
        {
            _service = service;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
        [HasPermission("Bilties.View")]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            BiltyStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search, status, dateFrom, dateTo);

            return Ok(new
            {
                success = true,
                message = "Bilties retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("Bilties.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var bilty = await _service.GetByIdAsync(id);

            if (bilty is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Bilty not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Bilty retrieved successfully.",
                data = bilty
            });
        }

        [HttpPost]
        [HasPermission("Bilties.Create")]
        public async Task<IActionResult> Create(BiltyCreateDto dto)
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
                    message = "Bilty created successfully.",
                    data = created
                });
        }

        [HttpPut("{id}")]
        [HasPermission("Bilties.Update")]
        public async Task<IActionResult> Update(int id, BiltyUpdateDto dto)
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
                    message = "Bilty not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Bilty updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [HasPermission("Bilties.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Bilty not found."
                });
            }

            return NoContent();
        }

        [HttpGet("{id}/print")]
        [HasPermission("Bilties.Print")]
        public async Task<IActionResult> Print(int id)
        {
            var pdf = await _service.GeneratePrintPdfAsync(id);

            if (pdf is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Bilty not found."
                });
            }

            return File(pdf, "application/pdf", $"Bilty-{id}.pdf");
        }
    }
}