using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.DTOs.Payment;
using ZMAllied.Application.Interfaces;
using ZMAllied.Domain.Enums;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/payments")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        [HttpGet]
        [HasPermission("Payments.View")]
        public async Task<IActionResult> GetAll(
            int pageNumber = 1,
            int pageSize = 10,
            string? search = null,
            PaymentStatus? status = null,
            DateTime? dateFrom = null,
            DateTime? dateTo = null)
        {
            var result = await _paymentService.GetPagedAsync(
                pageNumber,
                pageSize,
                search,
                status,
                dateFrom,
                dateTo);

            return Ok(new
            {
                success = true,
                data = result
            });
        }

        [HttpGet("{id}")]
        [HasPermission("Payments.View")]
        public async Task<IActionResult> GetById(int id)
        {
            var payment = await _paymentService.GetByIdAsync(id);

            if (payment is null)
            {
                return NotFound();
            }

            return Ok(new
            {
                success = true,
                data = payment
            });
        }

        [HttpPost]
        [HasPermission("Payments.Create")]
        public async Task<IActionResult> Create(PaymentCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            var created = await _paymentService.CreateAsync(dto, userId);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }

        [HttpPut("{id}")]
        [HasPermission("Payments.Update")]
        public async Task<IActionResult> Update(int id, PaymentUpdateDto dto)
        {
            var userId = GetUserId();
            var updated = await _paymentService.UpdateAsync(id, dto, userId);

            if (!updated)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        [HasPermission("Payments.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();
            var deleted = await _paymentService.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}