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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public PrescriptionController(
            IPrescriptionRepo prescriptionRepo,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            _prescriptionRepo = prescriptionRepo;
            _userManager = userManager;
            _environment = environment;
        }


        // CUSTOMER - VIEW OWN PRESCRIPTIONS
        [Authorize(Roles = "Customer")]
        public IActionResult Index()
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            var prescriptionList =
                _prescriptionRepo.GetPrescriptionsByUserId(userId);

            return View(prescriptionList);
        }


        // CUSTOMER - OPEN UPLOAD PAGE
        [HttpGet]
        [Authorize(Roles = "Customer")]
        public IActionResult Upload()
        {
            return View();
        }


        // CUSTOMER - UPLOAD PRESCRIPTION
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public IActionResult Upload(IFormFile prescriptionFile)
        {
            if (prescriptionFile == null || prescriptionFile.Length == 0)
            {
                TempData["Error"] = "Please select a prescription file.";
                return View();
            }


            // ALLOWED FILE TYPES
            string[] allowedExtensions =
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".pdf"
            };

            string fileExtension =
                Path.GetExtension(prescriptionFile.FileName)
                    .ToLowerInvariant();


            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["Error"] =
                    "Only JPG, JPEG, PNG and PDF files are allowed.";

                return View();
            }


            // MAXIMUM FILE SIZE = 5 MB
            long maximumFileSize = 5 * 1024 * 1024;

            if (prescriptionFile.Length > maximumFileSize)
            {
                TempData["Error"] =
                    "Prescription file size cannot exceed 5 MB.";

                return View();
            }


            // CREATE PRESCRIPTION FOLDER
            string uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                "prescriptions");


            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }


            // GENERATE UNIQUE FILE NAME
            string uniqueFileName =
                Guid.NewGuid().ToString() + fileExtension;


            string physicalFilePath = Path.Combine(
                uploadsFolder,
                uniqueFileName);


            // SAVE FILE
            using (var fileStream = new FileStream(
                physicalFilePath,
                FileMode.Create))
            {
                prescriptionFile.CopyTo(fileStream);
            }


            // GET CURRENT LOGGED-IN USER
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }


            // SAVE PRESCRIPTION IN DATABASE
            Prescription prescription = new Prescription
            {
                UserId = userId,
                FilePath = "/prescriptions/" + uniqueFileName,
                UploadDate = DateTime.Now,
                Status = "Pending"
            };


            int result =
                _prescriptionRepo.AddPrescription(prescription);


            if (result > 0)
            {
                TempData["Success"] =
                    "Prescription uploaded successfully.";

                return RedirectToAction(nameof(Index));
            }


            TempData["Error"] =
                "Prescription could not be uploaded.";

            return View();
        }

        [Authorize(Roles = "Customer")]
        public IActionResult Details(int id)
        {
            var prescription =
                _prescriptionRepo.GetPrescriptionById(id);

            if (prescription == null)
            {
                return NotFound();
            }

            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (prescription.UserId != userId)
            {
                return Forbid();
            }

            return View(prescription);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Review(int id)
        {
            var prescription =
                _prescriptionRepo.GetPrescriptionById(id);

            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id, string? adminRemarks)
        {
            var prescription =
                _prescriptionRepo.GetPrescriptionById(id);

            if (prescription == null)
            {
                return NotFound();
            }

            prescription.Status = "Approved";
            prescription.AdminRemarks = adminRemarks;

            _prescriptionRepo.UpdatePrescriptionStatus(
    id,
    "Approved",
    adminRemarks);

            return RedirectToAction(nameof(Review), new { id });
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Reject(int id, string? adminRemarks)
        {
            var prescription =
                _prescriptionRepo.GetPrescriptionById(id);

            if (prescription == null)
            {
                return NotFound();
            }

            prescription.Status = "Rejected";
            prescription.AdminRemarks = adminRemarks;

            _prescriptionRepo.UpdatePrescriptionStatus(
                id,
                "Rejected",adminRemarks);

            return RedirectToAction(nameof(Review), new { id });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ManagePrescriptions()
        {
            var prescriptionList =
                _prescriptionRepo.GetAllPrescriptions();

            return View(prescriptionList);
        }
    }
}
