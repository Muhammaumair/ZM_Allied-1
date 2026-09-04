using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Supplier;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        /// <summary>
        /// Get paginated list of suppliers with optional search and active filtering.
        /// </summary>
        [HttpGet]
        [HasPermission("Suppliers.View")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] bool? isActive = null)
        {
            var result = await _supplierService.GetPagedAsync(pageNumber, pageSize, search, isActive);
            return Ok(new { success = true, message = "Suppliers retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get a supplier by ID.
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission("Suppliers.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound(new { success = false, message = "Supplier not found." });

            return Ok(new { success = true, message = "Supplier retrieved successfully.", data = supplier });
        }

        /// <summary>
        /// Create a new supplier.
        /// </summary>
        [HttpPost]
        [HasPermission("Suppliers.Create")]
        public async Task<IActionResult> Create([FromBody] SupplierCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var created = await _supplierService.CreateAsync(dto, GetCurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, message = "Supplier created successfully.", data = created });
        }

        /// <summary>
        /// Update an existing supplier.
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission("Suppliers.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] SupplierUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _supplierService.UpdateAsync(id, dto, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Supplier not found." });

            return Ok(new { success = true, message = "Supplier updated successfully." });
        }

        /// <summary>
        /// Soft delete a supplier.
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission("Suppliers.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _supplierService.DeleteAsync(id, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Supplier not found." });

            return NoContent();
        }
    }
}
