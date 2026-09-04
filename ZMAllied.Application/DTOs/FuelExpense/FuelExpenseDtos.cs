using System.ComponentModel.DataAnnotations;

namespace ZMAllied.Application.DTOs.FuelExpense
{
    public class FuelExpenseCreateDto
    {
        [Range(1, int.MaxValue)]
        public int VehicleId { get; set; }

        public int? TripId { get; set; }

        public DateTime Date { get; set; }

        [MaxLength(50)]
        public string? FuelType { get; set; }

        public decimal Quantity { get; set; }

        public decimal Rate { get; set; }

        public decimal Odometer { get; set; }

        public int? SupplierId { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
    }

    public class FuelExpenseUpdateDto : FuelExpenseCreateDto
    {
    }

    public class FuelExpenseResponseDto
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int? TripId { get; set; }
        public DateTime Date { get; set; }
        public string? FuelType { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public decimal Odometer { get; set; }
        public int? SupplierId { get; set; }
        public string? Remarks { get; set; }
    }
}