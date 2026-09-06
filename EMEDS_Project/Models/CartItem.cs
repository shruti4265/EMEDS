namespace EMEDS_Project.Models
{
    public class CartItem
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
