namespace ZMAllied.Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        // Entity Record Counts
        public int TotalParties { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalDrivers { get; set; }
        public int TotalTrips { get; set; }
        public int TotalBilties { get; set; }
        public int TotalDDRs { get; set; }
        public int TotalCashBookEntries { get; set; }
        public int TotalPayments { get; set; }
        public int TotalReceipts { get; set; }
        public int TotalFuelExpenses { get; set; }
        public int TotalVehicleMaintenances { get; set; }

        // Financial & Operational Totals
        public decimal TotalFreight { get; set; }
        public decimal TotalAdvance { get; set; }
        public decimal TotalBalanceDue { get; set; }
        public decimal TotalPaymentsAmount { get; set; }
        public decimal TotalReceiptsAmount { get; set; }
        public decimal TotalFuelExpensesAmount { get; set; }
        public decimal TotalMaintenanceExpensesAmount { get; set; }
    }
}
