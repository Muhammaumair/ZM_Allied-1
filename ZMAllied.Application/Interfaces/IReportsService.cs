using System;
using System.Threading.Tasks;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Reports;

namespace ZMAllied.Application.Interfaces
{
    public interface IReportsService
    {
        Task<PagedResult<TripReportDto>> GetTripsReportAsync(int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search, string? status);
        Task<PagedResult<BiltyReportDto>> GetBiltiesReportAsync(int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search, string? status);
        Task<PagedResult<PaymentReportDto>> GetPaymentsReportAsync(int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search, string? status);
        Task<PagedResult<ReceiptReportDto>> GetReceiptsReportAsync(int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search, string? status);
        Task<PagedResult<ExpenseReportDto>> GetExpensesReportAsync(int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search);
        Task<PagedResult<VehicleMaintenanceReportDto>> GetVehicleMaintenanceReportAsync(int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search);
        Task<FinancialSummaryReportDto> GetFinancialSummaryAsync(DateTime? startDate, DateTime? endDate);
        Task<PagedResult<VehicleReportDto>> GetVehiclesReportAsync(int pageNumber, int pageSize, string? search);
    }
}
