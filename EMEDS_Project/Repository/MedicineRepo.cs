using EMEDS_Project.DAL;
using EMEDS_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace EMEDS_Project.Repository
{
    public class MedicineRepo : IMedicineRepo
    {
        private readonly EmedDbContext _context;

        public MedicineRepo(EmedDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Medicine> GetAllMedicines()
        {
            return _context.Medicines
                .AsNoTracking()
                .ToList();
        }

        public Medicine? GetMedicineById(int medicineId)
        {
            return _context.Medicines
                .AsNoTracking()
                .FirstOrDefault(m => m.MedicineId == medicineId);
        }

        public void AddMedicine(Medicine medicine)
        {
            _context.Medicines.Add(medicine);
            _context.SaveChanges();
        }

        public void UpdateMedicine(Medicine medicine)
        {
            _context.Medicines.Update(medicine);
            _context.SaveChanges();
        }

        public void DeleteMedicine(int medicineId)
        {
            Medicine? medicine = _context.Medicines
                .FirstOrDefault(m => m.MedicineId == medicineId);

            if (medicine != null)
            {
                _context.Medicines.Remove(medicine);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Medicine> SearchMedicines(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllMedicines();
            }

            return _context.Medicines
                .AsNoTracking()
                .Where(m => m.MedicineName.Contains(searchTerm))
                .ToList();
        }

        public IEnumerable<Medicine> GetMedicinesByCategory(int categoryId)
        {
            return _context.Medicines
                .AsNoTracking()
                .Where(m => m.CategoryId == categoryId)
                .ToList();
        }
    }
}