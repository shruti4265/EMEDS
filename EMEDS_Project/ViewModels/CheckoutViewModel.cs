using System.ComponentModel.DataAnnotations;
using EMEDS_Project.Models;

namespace EMEDS_Project.ViewModels
{
    public class CheckoutViewModel
    {
        [Required, StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required, StringLength(300)]
        public string Address { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public List<CartItem> CartItems { get; set; } = new();

        public decimal TotalAmount { get; set; }
    }
}
