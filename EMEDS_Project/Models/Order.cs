using EMEDS_Project.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMEDS_Project.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "User is required.")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order date is required.")]
        [DataType(DataType.DateTime)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, 999999.99,
            ErrorMessage = "Total amount must be greater than 0.")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Delivery address is required.")]
        [StringLength(500, MinimumLength = 10,
            ErrorMessage = "Delivery address must be between 10 and 500 characters.")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order status is required.")]
        [StringLength(50)]
        [RegularExpression(
            "Pending|Confirmed|Packed|Shipped|OutForDelivery|Delivered|Cancelled",
            ErrorMessage = "Invalid order status.")]
        public string OrderStatus { get; set; } = "Pending";

        public ApplicationUser? User { get; set; }
        public Payment? Payment { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
    }
}