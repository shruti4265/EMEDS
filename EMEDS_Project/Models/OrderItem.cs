using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMEDS_Project.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int MedicineId { get; set; }

        public Medicine? Medicine { get; set; }

        [Required]
        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        public Order? Order { get; set; }

        public decimal CalculateSubtotal()
        {
            return Quantity * UnitPrice;
        }
    }
}