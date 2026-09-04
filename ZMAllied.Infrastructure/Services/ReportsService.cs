using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZMAllied.Application.Common;
using ZMAllied.Application.DTOs.Reports;
using ZMAllied.Application.Interfaces;
using ZMAllied.Infrastructure.Persistence;

namespace ZMAllied.Infrastructure.Services
{
    public class ReportsService : IReportsService
    {
        private readonly ZMAlliedDbContext _context;

        public ReportsService(ZMAlliedDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<TripReportDto>> GetTripsReportAsync(
            int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search, string? status)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Trips.AsNoTracking().Where(t => !t.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(t => t.TripDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.TripDate <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Enums.TripStatus>(status, true, out var parsedStatus))
                query = query.Where(t => t.Status == parsedStatus);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(t =>
                    t.TripNumber.ToLower().Contains(s) ||
                    (t.Party != null && t.Party.Name.ToLower().Contains(s)) ||
                    (t.Vehicle != null && t.Vehicle.RegistrationNumber.ToLower().Contains(s)) ||
                    (t.Driver != null && t.Driver.Name.ToLower().Contains(s)) ||
                    (t.LoadingPoint != null && t.LoadingPoint.ToLower().Contains(s)) ||
                    (t.OffloadingPoint != null && t.OffloadingPoint.ToLower().Contains(s)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.TripDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TripReportDto
                {
                    Id = t.Id,
                    TripNumber = t.TripNumber,
                    CompanyId = t.CompanyId,
                    PartyId = t.PartyId,
                    PartyName = t.Party.Name,
                    VehicleId = t.VehicleId,
                    VehicleNumber = t.Vehicle.RegistrationNumber,
                    DriverId = t.DriverId,
                    DriverName = t.Driver.Name,
                    TripDate = t.TripDate,
                    LoadingPoint = t.LoadingPoint,
                    OffloadingPoint = t.OffloadingPoint,
                    Status = t.Status.ToString(),
                    TotalFreight = t.TotalFreight,
                    Advance = t.Advance,
                    Balance = t.Balance
                })
                .ToListAsync();

            return new PagedResult<TripReportDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<BiltyReportDto>> GetBiltiesReportAsync(
            int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search, string? status)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Bilties.AsNoTracking().Where(b => !b.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(b => b.BiltyDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(b => b.BiltyDate <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Enums.BiltyStatus>(status, true, out var parsedStatus))
                query = query.Where(b => b.Status == parsedStatus);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(b =>
                    b.BiltyNumber.ToLower().Contains(s) ||
                    (b.Vehicle != null && b.Vehicle.RegistrationNumber.ToLower().Contains(s)) ||
                    (b.Driver != null && b.Driver.Name.ToLower().Contains(s)) ||
                    (b.Consignor != null && b.Consignor.Name.ToLower().Contains(s)) ||
                    (b.Consignee != null && b.Consignee.Name.ToLower().Contains(s)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(b => b.BiltyDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BiltyReportDto
                {
                    Id = b.Id,
                    BiltyNumber = b.BiltyNumber,
                    BiltyDate = b.BiltyDate,
                    OfficeId = b.OfficeId,
                    VehicleId = b.VehicleId,
                    VehicleNumber = b.Vehicle.RegistrationNumber,
                    DriverId = b.DriverId,
                    DriverName = b.Driver.Name,
                    ConsignorName = b.Consignor != null ? b.Consignor.Name : null,
                    ConsigneeName = b.Consignee != null ? b.Consignee.Name : null,
                    LoadingPoint = b.LoadingPoint,
                    OffloadingPoint = b.OffloadingPoint,
                    Freight = b.Freight,
                    Advance = b.Advance,
                    Balance = b.Balance,
                    Status = b.Status.ToString()
                })
                .ToListAsync();

            return new PagedResult<BiltyReportDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<PaymentReportDto>> GetPaymentsReportAsync(
            int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search, string? status)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Payments.AsNoTracking().Where(p => !p.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(p => p.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(p => p.Date <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Enums.PaymentStatus>(status, true, out var parsedStatus))
                query = query.Where(p => p.Status == parsedStatus);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(p =>
                    p.PaymentNumber.ToLower().Contains(s) ||
                    (p.Party != null && p.Party.Name.ToLower().Contains(s)) ||
                    (p.Supplier != null && p.Supplier.Name.ToLower().Contains(s)) ||
                    (p.ReferenceNumber != null && p.ReferenceNumber.ToLower().Contains(s)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(p => p.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PaymentReportDto
                {
                    Id = p.Id,
                    PaymentNumber = p.PaymentNumber,
                    Date = p.Date,
                    PartyId = p.PartyId,
                    PartyName = p.Party != null ? p.Party.Name : null,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier != null ? p.Supplier.Name : null,
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod,
                    ReferenceNumber = p.ReferenceNumber,
                    Status = p.Status.ToString()
                })
                .ToListAsync();

            return new PagedResult<PaymentReportDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<ReceiptReportDto>> GetReceiptsReportAsync(
            int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search, string? status)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Receipts.AsNoTracking().Where(r => !r.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(r => r.Date >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.Date <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<Domain.Enums.PaymentStatus>(status, true, out var parsedStatus))
                query = query.Where(r => r.Status == parsedStatus);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(r =>
                    r.ReceiptNumber.ToLower().Contains(s) ||
                    (r.Party != null && r.Party.Name.ToLower().Contains(s)) ||
                    (r.ReferenceNumber != null && r.ReferenceNumber.ToLower().Contains(s)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new ReceiptReportDto
                {
                    Id = r.Id,
                    ReceiptNumber = r.ReceiptNumber,
                    Date = r.Date,
                    PartyId = r.PartyId,
                    PartyName = r.Party.Name,
                    Amount = r.Amount,
                    PaymentMethod = r.PaymentMethod,
                    ReferenceNumber = r.ReferenceNumber,
                    Status = r.Status.ToString()
                })
                .ToListAsync();

            return new PagedResult<ReceiptReportDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<ExpenseReportDto>> GetExpensesReportAsync(
            int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var fuelQuery = _context.FuelExpenses.AsNoTracking().Where(f => !f.IsDeleted);
            var maintQuery = _context.VehicleMaintenances.AsNoTracking().Where(m => !m.IsDeleted);

            if (startDate.HasValue)
            {
                fuelQuery = fuelQuery.Where(f => f.Date >= startDate.Value);
                maintQuery = maintQuery.Where(m => m.MaintenanceDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                fuelQuery = fuelQuery.Where(f => f.Date <= endDate.Value);
                maintQuery = maintQuery.Where(m => m.MaintenanceDate <= endDate.Value);
            }

            var fuelProjected = fuelQuery.Select(f => new ExpenseReportDto
            {
                Category = "Fuel",
                Id = f.Id,
                Date = f.Date,
                VehicleId = f.VehicleId,
                VehicleNumber = f.Vehicle.RegistrationNumber,
                SupplierId = f.SupplierId,
                SupplierName = f.Supplier != null ? f.Supplier.Name : null,
                Amount = f.Amount,
                Description = f.Remarks
            });

            var maintProjected = maintQuery.Select(m => new ExpenseReportDto
            {
                Category = "Maintenance",
                Id = m.Id,
                Date = m.MaintenanceDate,
                VehicleId = m.VehicleId,
                VehicleNumber = m.Vehicle.RegistrationNumber,
                SupplierId = m.SupplierId,
                SupplierName = m.Supplier != null ? m.Supplier.Name : null,
                Amount = m.Cost,
                Description = m.Description
            });

            var combined = fuelProjected.Concat(maintProjected);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                combined = combined.Where(e =>
                    e.VehicleNumber != null && e.VehicleNumber.ToLower().Contains(s) ||
                    (e.SupplierName != null && e.SupplierName.ToLower().Contains(s)) ||
                    (e.Description != null && e.Description.ToLower().Contains(s)));
            }

            var totalCount = await combined.CountAsync();

            var items = await combined
                .OrderByDescending(e => e.Date)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ExpenseReportDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResult<VehicleMaintenanceReportDto>> GetVehicleMaintenanceReportAsync(
            int pageNumber, int pageSize, DateTime? startDate, DateTime? endDate, string? search)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.VehicleMaintenances.AsNoTracking().Where(m => !m.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(m => m.MaintenanceDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.MaintenanceDate <= endDate.Value);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(m =>
                    (m.Vehicle != null && m.Vehicle.RegistrationNumber.ToLower().Contains(s)) ||
                    (m.MaintenanceType != null && m.MaintenanceType.ToLower().Contains(s)) ||
                    (m.Description != null && m.Description.ToLower().Contains(s)) ||
                    (m.Supplier != null && m.Supplier.Name.ToLower().Contains(s)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.MaintenanceDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new VehicleMaintenanceReportDto
                {
                    Id = m.Id,
                    VehicleId = m.VehicleId,
                    VehicleNumber = m.Vehicle.RegistrationNumber,
                    MaintenanceDate = m.MaintenanceDate,
                    MaintenanceType = m.MaintenanceType,
                    Description = m.Description,
                    Mileage = m.Mileage,
                    Cost = m.Cost,
                    SupplierId = m.SupplierId,
                    SupplierName = m.Supplier != null ? m.Supplier.Name : null,
                    NextMaintenanceDate = m.NextMaintenanceDate
                })
                .ToListAsync();

            return new PagedResult<VehicleMaintenanceReportDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<FinancialSummaryReportDto> GetFinancialSummaryAsync(DateTime? startDate, DateTime? endDate)
        {
            var paymentsQuery = _context.Payments.AsNoTracking().Where(p => !p.IsDeleted);
            var receiptsQuery = _context.Receipts.AsNoTracking().Where(r => !r.IsDeleted);
            var cashBookQuery = _context.CashBookEntries.AsNoTracking().Where(c => !c.IsDeleted);
            var fuelQuery = _context.FuelExpenses.AsNoTracking().Where(f => !f.IsDeleted);
            var maintQuery = _context.VehicleMaintenances.AsNoTracking().Where(m => !m.IsDeleted);

            if (startDate.HasValue)
            {
                paymentsQuery = paymentsQuery.Where(p => p.Date >= startDate.Value);
                receiptsQuery = receiptsQuery.Where(r => r.Date >= startDate.Value);
                cashBookQuery = cashBookQuery.Where(c => c.Date >= startDate.Value);
                fuelQuery = fuelQuery.Where(f => f.Date >= startDate.Value);
                maintQuery = maintQuery.Where(m => m.MaintenanceDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                paymentsQuery = paymentsQuery.Where(p => p.Date <= endDate.Value);
                receiptsQuery = receiptsQuery.Where(r => r.Date <= endDate.Value);
                cashBookQuery = cashBookQuery.Where(c => c.Date <= endDate.Value);
                fuelQuery = fuelQuery.Where(f => f.Date <= endDate.Value);
                maintQuery = maintQuery.Where(m => m.MaintenanceDate <= endDate.Value);
            }

            var totalPayments = await paymentsQuery.SumAsync(p => (decimal?)p.Amount) ?? 0m;
            var totalReceipts = await receiptsQuery.SumAsync(r => (decimal?)r.Amount) ?? 0m;
            var totalCashDebit = await cashBookQuery.SumAsync(c => (decimal?)c.Debit) ?? 0m;
            var totalCashCredit = await cashBookQuery.SumAsync(c => (decimal?)c.Credit) ?? 0m;
            var totalFuel = await fuelQuery.SumAsync(f => (decimal?)f.Amount) ?? 0m;
            var totalMaint = await maintQuery.SumAsync(m => (decimal?)m.Cost) ?? 0m;

            var netBalance = totalReceipts + totalCashDebit - totalPayments - totalCashCredit - totalFuel - totalMaint;

            return new FinancialSummaryReportDto
            {
                TotalPayments = totalPayments,
                TotalReceipts = totalReceipts,
                TotalCashBookDebit = totalCashDebit,
                TotalCashBookCredit = totalCashCredit,
                TotalFuelExpenses = totalFuel,
                TotalVehicleMaintenance = totalMaint,
                NetBalance = netBalance
            };
        }

        public async Task<PagedResult<VehicleReportDto>> GetVehiclesReportAsync(int pageNumber, int pageSize, string? search)
        {
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = pageSize < 1 ? 10 : (pageSize > 100 ? 100 : pageSize);

            var query = _context.Vehicles.AsNoTracking().Where(v => !v.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                query = query.Where(v =>
                    v.RegistrationNumber.ToLower().Contains(s) ||
                    (v.Make != null && v.Make.ToLower().Contains(s)) ||
                    (v.Model != null && v.Model.ToLower().Contains(s)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(v => v.RegistrationNumber)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(v => new VehicleReportDto
                {
                    VehicleId = v.Id,
                    RegistrationNumber = v.RegistrationNumber,
                    MakeModel = ((v.Make ?? "") + " " + (v.Model ?? "")).Trim(),
                    TotalTrips = _context.Trips.Count(t => !t.IsDeleted && t.VehicleId == v.Id),
                    TotalFuelExpenses = _context.FuelExpenses.Where(f => !f.IsDeleted && f.VehicleId == v.Id).Sum(f => (decimal?)f.Amount) ?? 0m,
                    TotalFuelQuantity = _context.FuelExpenses.Where(f => !f.IsDeleted && f.VehicleId == v.Id).Sum(f => (decimal?)f.Quantity) ?? 0m,
                    TotalMaintenanceCost = _context.VehicleMaintenances.Where(m => !m.IsDeleted && m.VehicleId == v.Id).Sum(m => (decimal?)m.Cost) ?? 0m,
                    LastMaintenanceDate = _context.VehicleMaintenances.Where(m => !m.IsDeleted && m.VehicleId == v.Id).Max(m => (DateTime?)m.MaintenanceDate)
                })
                .ToListAsync();

            return new PagedResult<VehicleReportDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }
    }
}
