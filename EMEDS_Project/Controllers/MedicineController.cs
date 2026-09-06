using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    [Authorize]
    public class MedicineController : Controller
    {
        private readonly IMedicineRepo _medicineRepo;

        public MedicineController(IMedicineRepo medicineRepo)
        {
            _medicineRepo = medicineRepo;
        }

        // GET: Medicine
        public IActionResult Index()
        {
            IEnumerable<Medicine> medicines =
                _medicineRepo.GetAllMedicines();

            return View(medicines);
        }

        // GET: Medicine/Details/5
        public IActionResult Details(int id)
        {
            Medicine? medicine =
                _medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // GET: Medicine/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medicine/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(Medicine medicine)
        {
            if (!ModelState.IsValid)
            {
                return View(medicine);
            }

            _medicineRepo.AddMedicine(medicine);

            return RedirectToAction(nameof(Index));
        }

        // GET: Medicine/Edit/5
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id)
        {
            Medicine? medicine =
                _medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // POST: Medicine/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit(int id, Medicine medicine)
        {
            if (id != medicine.MedicineId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(medicine);
            }

            _medicineRepo.UpdateMedicine(medicine);

            return RedirectToAction(nameof(Index));
        }

        // GET: Medicine/Delete/5
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            Medicine? medicine =
                _medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return NotFound();
            }

            return View(medicine);
        }

        // POST: Medicine/Delete/5
        [HttpPost, ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteConfirmed(int id)
        {
            Medicine? medicine =
                _medicineRepo.GetMedicineById(id);

            if (medicine == null)
            {
                return NotFound();
            }

            _medicineRepo.DeleteMedicine(id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Medicine/Search
        public IActionResult Search(string searchTerm)
        {
            IEnumerable<Medicine> medicines =
                _medicineRepo.SearchMedicines(searchTerm);

            return View("Index", medicines);
        }
    }
}