



using EMEDS_Project.Models;

namespace EMEDS_Project.Repository
{
    public interface IPrescriptionRepo
    {
        List<Prescription> GetAllPrescriptions();

        Prescription? GetPrescriptionById(int prescriptionId);

        List<Prescription> GetPrescriptionsByUserId(string userId);

        bool HasApprovedPrescriptionForMedicine(
            string userId,
            int medicineId);

        Prescription? GetPrescriptionForOrderMedicine(
            int orderId,
            int medicineId,
            string userId);

        int AddPrescription(Prescription prescription);

        int UpdatePrescriptionStatus(
            int prescriptionId,
            string status,
            string? adminRemarks);
    }
}