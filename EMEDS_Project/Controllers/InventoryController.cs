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

        public InventoryController(IInventoryRepo inventoryRepo, ISupplierRepo supplierRepo)
        {
            _inventoryRepo = inventoryRepo;
            _supplierRepo = supplierRepo;
        }

        public IActionResult Index()
        {
            var inventory = _inventoryRepo.GetAllWithSupplier();
            return View(inventory);
        }

        public IActionResult Details(int id)
        {
            var inventory = _inventoryRepo.GetById(id);

            if (inventory == null)
            {
                return NotFound();
            }

            return View(inventory);
        }

        public IActionResult Create()
        {
            LoadSuppliers();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inventory inventory)
        {
            var existingInventory = _inventoryRepo.GetByMedicineId(inventory.MedicineId);

            if (existingInventory != null)
            {
                ModelState.AddModelError("MedicineId", "Inventory already exists for this medicine.");
            }

            if (ModelState.IsValid)
            {
                inventory.LastUpdated = DateTime.Now;
                _inventoryRepo.Add(inventory);
                _inventoryRepo.Save();
                return RedirectToAction(nameof(Index));
            }

            LoadSuppliers();
            return View(inventory);
        }

        public IActionResult Edit(int id)
        {
            var inventory = _inventoryRepo.GetById(id);

            if (inventory == null)
            {
                return NotFound();
            }

            LoadSuppliers();
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

            LoadSuppliers();
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

        private void LoadSuppliers()
        {
            ViewBag.Suppliers = new SelectList(
                _supplierRepo.GetAll(),
                "SupplierId",
                "SupplierName");
        }
    }
}