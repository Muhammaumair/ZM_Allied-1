using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Company;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompaniesController(ICompanyService service)
        {
            _service = service;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim is null ? null : int.Parse(userIdClaim.Value);
        }

        [HttpGet]
        [HasPermission("Companies.View")]
        public async Task<IActionResult> GetAll(int pageNumber = 1,int pageSize = 10,string? search = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search);

            return Ok(new
            {
                success = true,
                message = "Companies retrieved successfully.",
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("Companies.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var company = await _service.GetByIdAsync(id);

            if (company is null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Company not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Company retrieved successfully.",
                data = company
            });
        }

        [HttpPost]
        [HasPermission("Companies.Create")]
        public async Task<IActionResult> Create(CompanyCreateDto dto)
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

            var userId = GetCurrentUserId();
            var created = await _service.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                new
                {
                    success = true,
                    message = "Company created successfully.",
                    data = created
                });
        }

        [HttpPut("{id}")]
        [HasPermission("Companies.Update")]
        public async Task<IActionResult> Update(int id, CompanyUpdateDto dto)
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

            var userId = GetCurrentUserId();
            var updated = await _service.UpdateAsync(id, dto, userId);

            if (!updated)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Company not found."
                });
            }

            return Ok(new
            {
                success = true,
                message = "Company updated successfully."
            });
        }

        [HttpDelete("{id}")]
        [HasPermission("Companies.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            var deleted = await _service.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Company not found."
                });
            }

            return NoContent();
        }
    }
}