using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMEDS_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InventoryController : Controller
    {
        private readonly IInventoryRepo _inventoryRepo;
        private readonly ISupplierRepo _supplierRepo;
        private readonly IMedicineRepo _medicineRepo;

        public InventoryController(
            IInventoryRepo inventoryRepo,
            ISupplierRepo supplierRepo,
            IMedicineRepo medicineRepo)
        {
            _inventoryRepo = inventoryRepo;
            _supplierRepo = supplierRepo;
            _medicineRepo = medicineRepo;
        }

        public IActionResult Index()
        {
            var inventory = _inventoryRepo.GetAllWithSupplier();
            return View(inventory);
        }

        public IActionResult Details(int id)
        {
            var inventory = _inventoryRepo.GetByIdWithSupplier(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inventory inventory)
        {
            var medicine = _medicineRepo.GetMedicineById(inventory.MedicineId);

            if (medicine == null)
            {
                ModelState.AddModelError("MedicineId", "Selected medicine does not exist.");
            }
            else
            {
                var existingInventory = _inventoryRepo.GetByMedicineId(inventory.MedicineId);

                if (existingInventory != null)
                {
                    ModelState.AddModelError("MedicineId", "Inventory already exists for this medicine.");
                }
            }

            if (ModelState.IsValid)
            {
                inventory.LastUpdated = DateTime.Now;
                _inventoryRepo.Add(inventory);
                _inventoryRepo.Save();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(inventory);
        }

        public IActionResult Edit(int id)
        {
            var inventory = _inventoryRepo.GetById(id);

            if (inventory == null)
            {
                return NotFound();
            }

            LoadDropdowns();
            return View(inventory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Inventory inventory)
        {
            if (ModelState.IsValid)
            {
                inventory.LastUpdated = DateTime.Now;
                _inventoryRepo.Update(inventory);
                _inventoryRepo.Save();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(inventory);
        }

        public IActionResult LowStock()
        {
            var inventory = _inventoryRepo.GetLowStock();
            return View(inventory);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStock(int medicineId, int quantity)
        {
            _inventoryRepo.UpdateStock(medicineId, quantity);
            return RedirectToAction(nameof(Index));
        }

        private void LoadDropdowns()
        {
            ViewBag.Suppliers = new SelectList(
                _supplierRepo.GetAll(),
                "SupplierId",
                "SupplierName");

            ViewBag.Medicines = new SelectList(
                _medicineRepo.GetAllMedicines(),
                "MedicineId",
                "MedicineName");
        }
    }
}