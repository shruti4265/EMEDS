using EMEDS_Project.DAL;
using EMEDS_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace EMEDS_Project.Repository
{
    public class InventoryRepo : Repository<Inventory>, IInventoryRepo
    {
        public InventoryRepo(EmedDbContext context) : base(context)
        {
        }

        public IEnumerable<Inventory> GetAllWithSupplier()
        {
            return _context.Inventories
                .Include(i => i.Supplier)
                .ToList();
        }

        public IEnumerable<Inventory> GetLowStock()
        {
            return _context.Inventories
                .Where(i => i.StockQuantity <= i.ReorderLevel)
                .ToList();
        }

        public Inventory? GetByMedicineId(int medicineId)
        {
            return _context.Inventories
                .Include(i => i.Supplier)
                .FirstOrDefault(i => i.MedicineId == medicineId);
        }

        public void UpdateStock(int medicineId, int quantity)
        {
            var inventory = _context.Inventories
                .FirstOrDefault(i => i.MedicineId == medicineId);

            if (inventory != null)
            {
                inventory.StockQuantity = quantity;
                inventory.LastUpdated = DateTime.Now;
                _context.SaveChanges();
            }
        }
    }
}