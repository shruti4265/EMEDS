//using EMEDS_Project.Models;
//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;

//namespace EMEDS_Project.Controllers
//{
//    public class HomeController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }

//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}



using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EMEDS_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMedicineRepo _medicineRepo;
        private readonly ICategoryRepo _categoryRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IPrescriptionRepo _prescriptionRepo;
        private readonly IInventoryRepo _inventoryRepo;
        private readonly ISupplierRepo _supplierRepo;

        public HomeController(
            IMedicineRepo medicineRepo,
            ICategoryRepo categoryRepo,
            IOrderRepo orderRepo,
            IPrescriptionRepo prescriptionRepo,
            IInventoryRepo inventoryRepo,
            ISupplierRepo supplierRepo)
        {
            _medicineRepo = medicineRepo;
            _categoryRepo = categoryRepo;
            _orderRepo = orderRepo;
            _prescriptionRepo = prescriptionRepo;
            _inventoryRepo = inventoryRepo;
            _supplierRepo = supplierRepo;
        }

        public IActionResult Index()
        {
            // Admin dashboard statistics
            if (User.IsInRole("Admin"))
            {
                ViewBag.TotalMedicines =
                    _medicineRepo.GetAllMedicines().Count();

                ViewBag.TotalCategories =
                    _categoryRepo.GetAllCategories().Count();

                ViewBag.TotalOrders =
                    _orderRepo.GetAllOrders().Count;

                ViewBag.PendingPrescriptions =
                    _prescriptionRepo
                        .GetAllPrescriptions()
                        .Count(x => x.Status == "Pending");

                ViewBag.LowStockItems =
                    _inventoryRepo.GetLowStock().Count();

                ViewBag.TotalSuppliers =
                    _supplierRepo.GetAll().Count();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                            ?? HttpContext.TraceIdentifier
            });
        }
    }
}
