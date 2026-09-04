using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZMAllied.API.Extensions;
using ZMAllied.Application.Interfaces;

namespace ZMAllied.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _reportsService;

        public ReportsController(IReportsService reportsService)
        {
            _reportsService = reportsService;
        }

        /// <summary>
        /// Get paginated trip report with optional filters.
        /// </summary>
        [HttpGet("trips")]
        [HasPermission("Reports.View")]
        public async Task<IActionResult> GetTrips(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var result = await _reportsService.GetTripsReportAsync(pageNumber, pageSize, startDate, endDate, search, status);
            return Ok(new { success = true, message = "Trips report retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get paginated bilties report with optional filters.
        /// </summary>
        [HttpGet("bilties")]
        [HasPermission("Reports.View")]
        public async Task<IActionResult> GetBilties(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var result = await _reportsService.GetBiltiesReportAsync(pageNumber, pageSize, startDate, endDate, search, status);
            return Ok(new { success = true, message = "Bilties report retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get paginated payments report with optional filters.
        /// </summary>
        [HttpGet("payments")]
        [HasPermission("Reports.View")]
        public async Task<IActionResult> GetPayments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var result = await _reportsService.GetPaymentsReportAsync(pageNumber, pageSize, startDate, endDate, search, status);
            return Ok(new { success = true, message = "Payments report retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get paginated receipts report with optional filters.
        /// </summary>
        [HttpGet("receipts")]
        [HasPermission("Reports.View")]
        public async Task<IActionResult> GetReceipts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null,
            [FromQuery] string? status = null)
        {
            var result = await _reportsService.GetReceiptsReportAsync(pageNumber, pageSize, startDate, endDate, search, status);
            return Ok(new { success = true, message = "Receipts report retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get paginated expenses report (fuel + maintenance) with optional filters.
        /// </summary>
        [HttpGet("expenses")]
        [HasPermission("Reports.View")]
        public async Task<IActionResult> GetExpenses(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null)
        {
            var result = await _reportsService.GetExpensesReportAsync(pageNumber, pageSize, startDate, endDate, search);
            return Ok(new { success = true, message = "Expenses report retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get paginated vehicle maintenance report.
        /// </summary>
        [HttpGet("vehicle-maintenance")]
        [HasPermission("Reports.View")]
        public async Task<IActionResult> GetVehicleMaintenance(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? search = null)
        {
            var result = await _reportsService.GetVehicleMaintenanceReportAsync(pageNumber, pageSize, startDate, endDate, search);
            return Ok(new { success = true, message = "Vehicle maintenance report retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get overall financial summary.
        /// </summary>
        [HttpGet("financial-summary")]
        [HasPermission("Reports.View")]
        public async Task<IActionResult> GetFinancialSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var result = await _reportsService.GetFinancialSummaryAsync(startDate, endDate);
            return Ok(new { success = true, message = "Financial summary retrieved successfully.", data = result });
        }

        /// <summary>
        /// Get vehicle performance report.
        /// </summary>
        [HttpGet("vehicles")]
        [HasPermission("Reports.View")]
        public async Task<IActionResult> GetVehicles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _reportsService.GetVehiclesReportAsync(pageNumber, pageSize, search);
            return Ok(new { success = true, message = "Vehicle report retrieved successfully.", data = result });
        }
    }
}
