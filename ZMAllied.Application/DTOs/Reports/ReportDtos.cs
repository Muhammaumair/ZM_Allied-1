using System;

namespace ZMAllied.Application.DTOs.Reports
{
    public class TripReportDto
    {
        public int Id { get; set; }
        public string TripNumber { get; set; } = string.Empty;
        public int CompanyId { get; set; }
        public int PartyId { get; set; }
        public string? PartyName { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleNumber { get; set; }
        public int DriverId { get; set; }
        public string? DriverName { get; set; }
        public DateTime TripDate { get; set; }
        public string? LoadingPoint { get; set; }
        public string? OffloadingPoint { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalFreight { get; set; }
        public decimal Advance { get; set; }
        public decimal Balance { get; set; }
    }

    public class BiltyReportDto
    {
        public int Id { get; set; }
        public string BiltyNumber { get; set; } = string.Empty;
        public DateTime BiltyDate { get; set; }
        public int OfficeId { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleNumber { get; set; }
        public int DriverId { get; set; }
        public string? DriverName { get; set; }
        public string? ConsignorName { get; set; }
        public string? ConsigneeName { get; set; }
        public string? LoadingPoint { get; set; }
        public string? OffloadingPoint { get; set; }
        public decimal Freight { get; set; }
        public decimal Advance { get; set; }
        public decimal Balance { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PaymentReportDto
    {
        public int Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int? PartyId { get; set; }
        public string? PartyName { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ReceiptReportDto
    {
        public int Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public int PartyId { get; set; }
        public string? PartyName { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ExpenseReportDto
    {
        public string Category { get; set; } = string.Empty; // "Fuel" | "Maintenance"
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleNumber { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    public class VehicleMaintenanceReportDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleNumber { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string? MaintenanceType { get; set; }
        public string? Description { get; set; }
        public decimal Mileage { get; set; }
        public decimal Cost { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }

    public class FinancialSummaryReportDto
    {
        public decimal TotalPayments { get; set; }
        public decimal TotalReceipts { get; set; }
        public decimal TotalCashBookDebit { get; set; }
        public decimal TotalCashBookCredit { get; set; }
        public decimal TotalFuelExpenses { get; set; }
        public decimal TotalVehicleMaintenance { get; set; }
        public decimal NetBalance { get; set; }
    }

    public class VehicleReportDto
    {
        public int VehicleId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;
        public string MakeModel { get; set; } = string.Empty;
        public int TotalTrips { get; set; }
        public decimal TotalFuelExpenses { get; set; }
        public decimal TotalFuelQuantity { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
    }
}
