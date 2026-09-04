using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.DTOs.Dashboard;
using ZMAllied.Application.Interfaces;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ZMAlliedDbContext _context;

        public DashboardService(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var summary = new DashboardSummaryDto
            {
                TotalParties = await _context.Parties.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalSuppliers = await _context.Suppliers.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalVehicles = await _context.Vehicles.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalDrivers = await _context.Drivers.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalTrips = await _context.Trips.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalBilties = await _context.Bilties.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalDDRs = await _context.DDRs.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalCashBookEntries = await _context.CashBookEntries.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalPayments = await _context.Payments.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalReceipts = await _context.Receipts.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalFuelExpenses = await _context.FuelExpenses.AsNoTracking().CountAsync(x => !x.IsDeleted),
                TotalVehicleMaintenances = await _context.VehicleMaintenances.AsNoTracking().CountAsync(x => !x.IsDeleted),

                TotalFreight = await _context.Trips.AsNoTracking().Where(x => !x.IsDeleted).SumAsync(x => (decimal?)x.TotalFreight) ?? 0m,
                TotalAdvance = await _context.Trips.AsNoTracking().Where(x => !x.IsDeleted).SumAsync(x => (decimal?)x.Advance) ?? 0m,
                TotalBalanceDue = await _context.Trips.AsNoTracking().Where(x => !x.IsDeleted).SumAsync(x => (decimal?)x.Balance) ?? 0m,
                TotalPaymentsAmount = await _context.Payments.AsNoTracking().Where(x => !x.IsDeleted).SumAsync(x => (decimal?)x.Amount) ?? 0m,
                TotalReceiptsAmount = await _context.Receipts.AsNoTracking().Where(x => !x.IsDeleted).SumAsync(x => (decimal?)x.Amount) ?? 0m,
                TotalFuelExpensesAmount = await _context.FuelExpenses.AsNoTracking().Where(x => !x.IsDeleted).SumAsync(x => (decimal?)x.Amount) ?? 0m,
                TotalMaintenanceExpensesAmount = await _context.VehicleMaintenances.AsNoTracking().Where(x => !x.IsDeleted).SumAsync(x => (decimal?)x.Cost) ?? 0m
            };

            return summary;
        }
    }
}
