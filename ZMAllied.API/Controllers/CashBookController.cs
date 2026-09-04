using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.CashBook;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/cashbook")]
    [Authorize]
    public class CashBookController : ControllerBase
    {
        private readonly ICashBookService _service;

        public CashBookController(ICashBookService service)
        {
            _service = service;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
        [HasPermission("CashBook.View")]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null,
            string? entryType = null,
            string? referenceNumber = null)
        {
            var result = await _service.GetPagedAsync(
                pageNumber,
                pageSize,
                search,
                dateFrom,
                dateTo,
                entryType,
                referenceNumber);

            return Ok(new
            {
                success = true,
                message = "Cash book entries retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("CashBook.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var entry = await _service.GetByIdAsync(id);

            if (entry is null)
            {
                return NotFound(new { success = false, message = "Cash book entry not found." });
            }

            return Ok(new
            {
                success = true,
                message = "Cash book entry retrieved successfully.",
                data = entry
            });
        }

        [HttpPost]
        [HasPermission("CashBook.Create")]
        public async Task<IActionResult> Create(CashBookEntryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
            }

            var userId = GetUserId();
            var created = await _service.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new { success = true, message = "Cash book entry created successfully.", data = created });
        }

        [HttpPut("{id}")]
        [HasPermission("CashBook.Update")]
        public async Task<IActionResult> Update(int id, CashBookEntryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState });
            }

            var userId = GetUserId();
            var updated = await _service.UpdateAsync(id, dto, userId);

            if (!updated)
            {
                return NotFound(new { success = false, message = "Cash book entry not found." });
            }

            return Ok(new { success = true, message = "Cash book entry updated successfully." });
        }

        [HttpDelete("{id}")]
        [HasPermission("CashBook.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _service.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new { success = false, message = "Cash book entry not found." });
            }

            return NoContent();
        }
    }
}
