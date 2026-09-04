using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Receipt;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Enums;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReceiptsController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptsController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        /// <summary>
        /// Get paginated list of receipts with optional search and status filter.
        /// </summary>
        [HttpGet]
        [HasPermission("Receipts.View")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] PaymentStatus? status = null)
        {
            var result = await _receiptService.GetPagedAsync(pageNumber, pageSize, search, status);
            return Ok(new { success = true, message = "Receipts retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get a receipt by ID.
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission("Receipts.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var receipt = await _receiptService.GetByIdAsync(id);
            if (receipt == null)
                return NotFound(new { success = false, message = "Receipt not found." });

            return Ok(new { success = true, message = "Receipt retrieved successfully.", data = receipt });
        }

        /// <summary>
        /// Create a new receipt.
        /// </summary>
        [HttpPost]
        [HasPermission("Receipts.Create")]
        public async Task<IActionResult> Create([FromBody] ReceiptCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var created = await _receiptService.CreateAsync(dto, GetCurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, message = "Receipt created successfully.", data = created });
        }

        /// <summary>
        /// Update an existing receipt.
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission("Receipts.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] ReceiptUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _receiptService.UpdateAsync(id, dto, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Receipt not found." });

            return Ok(new { success = true, message = "Receipt updated successfully." });
        }

        /// <summary>
        /// Soft delete a receipt.
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission("Receipts.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _receiptService.DeleteAsync(id, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Receipt not found." });

            return NoContent();
        }
    }
}
