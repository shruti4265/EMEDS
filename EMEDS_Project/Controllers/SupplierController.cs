using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMEDS_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SupplierController : Controller
    {
        private readonly ISupplierRepo _supplierRepo;

        public SupplierController(ISupplierRepo supplierRepo)
        {
            _supplierRepo = supplierRepo;
        }

        // GET: Supplier
        public IActionResult Index()
        {
            var suppliers = _supplierRepo.GetAll();

            return View(suppliers);
        }

        // GET: Supplier/Details/5
        public IActionResult Details(int id)
        {
            var supplier = _supplierRepo.GetById(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // GET: Supplier/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _supplierRepo.Add(supplier);
                _supplierRepo.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }

        // GET: Supplier/Edit/5
        public IActionResult Edit(int id)
        {
            var supplier = _supplierRepo.GetById(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Supplier/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _supplierRepo.Update(supplier);
                _supplierRepo.Save();

                return RedirectToAction(nameof(Index));
            }

            return View(supplier);
        }

        // GET: Supplier/Delete/5
        public IActionResult Delete(int id)
        {
            var supplier = _supplierRepo.GetById(id);

            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        // POST: Supplier/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
          var supplier = _supplierRepo.GetById(id);

          if (supplier == null)
             return NotFound();

           try
           {
              _supplierRepo.Delete(supplier);
              _supplierRepo.Save();

              TempData["Success"] = "Supplier deleted successfully.";
           }
          catch (DbUpdateException)
          {
            TempData["Error"] =
                "This supplier cannot be deleted because it is linked to inventory records.";
          }

        return RedirectToAction(nameof(Index));
      }
    }
}
