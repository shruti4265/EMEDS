using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public interface IInventoryRepo : IRepository<Inventory>
    {
        IEnumerable<Inventory> GetAllWithSupplier();
        IEnumerable<Inventory> GetLowStock();
        Inventory? GetByMedicineId(int medicineId);
        void UpdateStock(int medicineId, int quantity);
    }
}
