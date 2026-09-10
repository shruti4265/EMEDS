

using EMEDS_Project.Data;
using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    [Authorize]
    public class PrescriptionController : Controller
    {
        private readonly IPrescriptionRepo _prescriptionRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public PrescriptionController(
            IPrescriptionRepo prescriptionRepo,
            IOrderRepo orderRepo,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _prescriptionRepo = prescriptionRepo;
            _orderRepo = orderRepo;
            _userManager = userManager;
            _environment = environment;
        }


        // ==============================
        // CUSTOMER - MY PRESCRIPTIONS
        // ==============================

        [Authorize(Roles = "Customer")]
        public IActionResult Index()
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            var prescriptions =
                _prescriptionRepo.GetPrescriptionsByUserId(userId);

            return View(prescriptions);
        }


        // ==============================
        // CUSTOMER - UPLOAD GET
        // ==============================

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public IActionResult Upload(int medicineId, int orderId)
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            var order = _orderRepo.GetOrderWithItems(orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Customer can upload only for their own order
            if (order.UserId != userId)
            {
                return Forbid();
            }

            // Medicine must exist in this order
            bool medicineExists =
                order.OrderItems.Any(
                    x => x.MedicineId == medicineId);

            if (!medicineExists)
            {
                return BadRequest();
            }

            // Prevent duplicate prescription
            var existingPrescription =
                _prescriptionRepo
                    .GetPrescriptionForOrderMedicine(
                        orderId,
                        medicineId,
                        userId);

            if (existingPrescription != null)
            {
                TempData["Error"] =
                    "Prescription has already been uploaded for this medicine.";

                return RedirectToAction(
                    "Details",
                    "Order",
                    new { id = orderId });
            }

            ViewBag.MedicineId = medicineId;
            ViewBag.OrderId = orderId;

            return View();
        }


        // ==============================
        // CUSTOMER - UPLOAD POST
        // ==============================

        [HttpPost]
        [Authorize(Roles = "Customer")]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(
            IFormFile prescriptionFile,
            int medicineId,
            int orderId)
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }


            // ---------- Validate Order ----------

            var order = _orderRepo.GetOrderWithItems(orderId);

            if (order == null)
            {
                return NotFound();
            }

            if (order.UserId != userId)
            {
                return Forbid();
            }


            // ---------- Validate Medicine ----------

            bool medicineExists =
                order.OrderItems.Any(
                    x => x.MedicineId == medicineId);

            if (!medicineExists)
            {
                return BadRequest();
            }


            // ---------- Prevent Duplicate ----------

            var existingPrescription =
                _prescriptionRepo
                    .GetPrescriptionForOrderMedicine(
                        orderId,
                        medicineId,
                        userId);

            if (existingPrescription != null)
            {
                TempData["Error"] =
                    "Prescription has already been uploaded for this medicine.";

                return RedirectToAction(
                    "Details",
                    "Order",
                    new { id = orderId });
            }


            // ---------- Validate File ----------

            if (prescriptionFile == null ||
                prescriptionFile.Length == 0)
            {
                TempData["Error"] =
                    "Please select a prescription file.";

                ViewBag.MedicineId = medicineId;
                ViewBag.OrderId = orderId;

                return View();
            }


            string extension =
                Path.GetExtension(
                    prescriptionFile.FileName)
                    .ToLowerInvariant();


            string[] allowedExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".pdf"
            };


            if (!allowedExtensions.Contains(extension))
            {
                TempData["Error"] =
                    "Only JPG, JPEG, PNG and PDF files are allowed.";

                ViewBag.MedicineId = medicineId;
                ViewBag.OrderId = orderId;

                return View();
            }


            // Maximum = 5 MB

            long maxFileSize =
                5 * 1024 * 1024;


            if (prescriptionFile.Length > maxFileSize)
            {
                TempData["Error"] =
                    "Prescription file cannot exceed 5 MB.";

                ViewBag.MedicineId = medicineId;
                ViewBag.OrderId = orderId;

                return View();
            }


            // ---------- Save File ----------

            string uploadFolder =
                Path.Combine(
                    _environment.WebRootPath,
                    "prescriptions");


            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }


            string uniqueFileName =
                Guid.NewGuid().ToString()
                + extension;


            string filePath =
                Path.Combine(
                    uploadFolder,
                    uniqueFileName);


            using (var stream =
                   new FileStream(
                       filePath,
                       FileMode.Create))
            {
                prescriptionFile.CopyTo(stream);
            }


            // ---------- Save Prescription ----------

            Prescription prescription =
                new Prescription
                {
                    UserId = userId,

                    MedicineId = medicineId,

                    OrderId = orderId,

                    FilePath =
                        "/prescriptions/" +
                        uniqueFileName,

                    UploadDate = DateTime.Now,

                    Status = "Pending"
                };


            int result =
                _prescriptionRepo
                    .AddPrescription(prescription);


            if (result > 0)
            {
                TempData["Success"] =
                    "Prescription uploaded successfully. Your order will continue normally.";

                return RedirectToAction(
                    "Details",
                    "Order",
                    new { id = orderId });
            }


            TempData["Error"] =
                "Prescription could not be uploaded.";

            ViewBag.MedicineId = medicineId;
            ViewBag.OrderId = orderId;

            return View();
        }


        // ==============================
        // CUSTOMER - DETAILS
        // ==============================

        [Authorize(Roles = "Customer")]
        public IActionResult Details(int id)
        {
            var prescription =
                _prescriptionRepo
                    .GetPrescriptionById(id);

            if (prescription == null)
            {
                return NotFound();
            }


            string? userId =
                _userManager.GetUserId(User);


            if (userId == null)
            {
                return Unauthorized();
            }


            // Customer cannot view another
            // customer's prescription

            if (prescription.UserId != userId)
            {
                return Forbid();
            }


            return View(prescription);
        }


        // ==============================
        // ADMIN - MANAGE PRESCRIPTIONS
        // ==============================

        [Authorize(Roles = "Admin")]
        public IActionResult ManagePrescriptions()
        {
            var prescriptions =
                _prescriptionRepo
                    .GetAllPrescriptions();

            return View(prescriptions);
        }


        // ==============================
        // ADMIN - REVIEW
        // ==============================

        [Authorize(Roles = "Admin")]
        public IActionResult Review(int id)
        {
            var prescription =
                _prescriptionRepo
                    .GetPrescriptionById(id);


            if (prescription == null)
            {
                return NotFound();
            }


            return View(prescription);
        }


        // ==============================
        // ADMIN - APPROVE
        // ==============================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Approve(
            int id,
            string? adminRemarks)
        {
            var prescription =
                _prescriptionRepo
                    .GetPrescriptionById(id);


            if (prescription == null)
            {
                return NotFound();
            }


            // Only pending prescription
            // should be reviewed

            if (prescription.Status != "Pending")
            {
                TempData["Error"] =
                    "This prescription has already been reviewed.";

                return RedirectToAction(
                    nameof(Review),
                    new { id });
            }


            _prescriptionRepo
                .UpdatePrescriptionStatus(
                    id,
                    "Approved",
                    adminRemarks);


            /*
             IMPORTANT:

             Approving a prescription DOES NOT
             change OrderStatus.

             The order continues normally.
            */


            TempData["Success"] =
                "Prescription approved successfully. The order will continue normally.";


            return RedirectToAction(
                nameof(Review),
                new { id });
        }


        // ==============================
        // ADMIN - REJECT
        // ==============================

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult Reject(
            int id,
            string? adminRemarks)
        {
            var prescription =
                _prescriptionRepo
                    .GetPrescriptionById(id);


            if (prescription == null)
            {
                return NotFound();
            }


            // Prevent multiple reviews

            if (prescription.Status != "Pending")
            {
                TempData["Error"] =
                    "This prescription has already been reviewed.";

                return RedirectToAction(
                    nameof(Review),
                    new { id });
            }


            // STEP 1:
            // Reject prescription

            _prescriptionRepo
                .UpdatePrescriptionStatus(
                    id,
                    "Rejected",
                    adminRemarks);


            // STEP 2:
            // Cancel ONLY the related active order

            if (prescription.OrderId.HasValue)
            {
                var order =
                    _orderRepo.GetOrderById(
                        prescription.OrderId.Value);


                if (order != null)
                {
                    /*
                     A delivered order should not
                     become cancelled afterwards.
                    */

                    if (order.OrderStatus != "Delivered" &&
                        order.OrderStatus != "Cancelled")
                    {
                        _orderRepo.UpdateOrderStatus(
                            order.OrderId,
                            "Cancelled");
                    }
                }
            }


            TempData["Success"] =
                "Prescription rejected. The related active order has been cancelled.";


            return RedirectToAction(
                nameof(Review),
                new { id });
        }
    }
}
