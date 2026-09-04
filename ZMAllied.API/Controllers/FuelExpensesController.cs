using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.FuelExpense;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FuelExpensesController : ControllerBase
    {
        private readonly IFuelExpenseService _fuelExpenseService;

        public FuelExpensesController(IFuelExpenseService fuelExpenseService)
        {
            _fuelExpenseService = fuelExpenseService;
        }

        private int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : null;
        }

        /// <summary>
        /// Get paginated list of fuel expenses with optional search.
        /// </summary>
        [HttpGet]
        [HasPermission("Fleet.View")]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _fuelExpenseService.GetPagedAsync(pageNumber, pageSize, search);
            return Ok(new { success = true, message = "Fuel expenses retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get a fuel expense by ID.
        /// </summary>
        [HttpGet("{id}")]
        [HasPermission("Fleet.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var expense = await _fuelExpenseService.GetByIdAsync(id);
            if (expense == null)
                return NotFound(new { success = false, message = "Fuel expense not found." });

            return Ok(new { success = true, message = "Fuel expense retrieved successfully.", data = expense });
        }

        /// <summary>
        /// Create a new fuel expense.
        /// </summary>
        [HttpPost]
        [HasPermission("Fleet.Create")]
        public async Task<IActionResult> Create([FromBody] FuelExpenseCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var created = await _fuelExpenseService.CreateAsync(dto, GetCurrentUserId());
            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                new { success = true, message = "Fuel expense created successfully.", data = created });
        }

        /// <summary>
        /// Update an existing fuel expense.
        /// </summary>
        [HttpPut("{id}")]
        [HasPermission("Fleet.Update")]
        public async Task<IActionResult> Update(int id, [FromBody] FuelExpenseUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { success = false, message = "Validation failed.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });

            var result = await _fuelExpenseService.UpdateAsync(id, dto, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Fuel expense not found." });

            return Ok(new { success = true, message = "Fuel expense updated successfully." });
        }

        /// <summary>
        /// Soft delete a fuel expense.
        /// </summary>
        [HttpDelete("{id}")]
        [HasPermission("Fleet.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _fuelExpenseService.DeleteAsync(id, GetCurrentUserId());
            if (!result)
                return NotFound(new { success = false, message = "Fuel expense not found." });

            return NoContent();
        }
    }
}
