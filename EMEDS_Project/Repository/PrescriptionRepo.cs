using EMEDS_Project.DAL;
using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public class PrescriptionRepo : IPrescriptionRepo
    {
        private readonly EmedDbContext _context;

        public PrescriptionRepo(EmedDbContext context)
        {
            _context = context;
        }

        public List<Prescription> GetAllPrescriptions()
        {
            return _context.Prescriptions.ToList();
        }

        public Prescription? GetPrescriptionById(int prescriptionId)
        {
            return _context.Prescriptions
                .FirstOrDefault(
                    prescription => prescription.PrescriptionId == prescriptionId);
        }

        public List<Prescription> GetPrescriptionsByUserId(string userId)
        {
            return _context.Prescriptions
                .Where(prescription => prescription.UserId == userId)
                .ToList();
        }

        public bool HasApprovedPrescriptionForMedicine(string userId, int medicineId)
        {
            return _context.Prescriptions
                .Any(p => p.UserId == userId
                       && p.MedicineId == medicineId
                       && p.Status == "Approved");
        }

        public int AddPrescription(Prescription prescription)
        {
            _context.Prescriptions.Add(prescription);

            return _context.SaveChanges();
        }

        public int UpdatePrescriptionStatus(
            int prescriptionId,
            string status,
            string? adminRemarks)
        {
            var prescription = _context.Prescriptions
                .FirstOrDefault(
                    prescription => prescription.PrescriptionId == prescriptionId);

            if (prescription == null)
            {
                return 0;
            }

            prescription.Status = status;
            prescription.AdminRemarks = adminRemarks;

            return _context.SaveChanges();
        }
    }
}