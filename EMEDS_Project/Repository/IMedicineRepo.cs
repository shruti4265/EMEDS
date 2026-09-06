using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public interface IMedicineRepo
    {
        IEnumerable<Medicine> GetAllMedicines();

        Medicine? GetMedicineById(int medicineId);

        void AddMedicine(Medicine medicine);

        void UpdateMedicine(Medicine medicine);

        void DeleteMedicine(int medicineId);

        IEnumerable<Medicine> SearchMedicines(string searchTerm);

        IEnumerable<Medicine> GetMedicinesByCategory(int categoryId);
    }
}