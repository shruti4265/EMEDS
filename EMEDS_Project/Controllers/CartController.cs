using EMEDS_Project.Helpers;
using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EMEDS_Project.Controllers
{
    [Authorize(Roles = "Customer")]
    public class CartController : Controller
    {
        private readonly IInventoryRepo _inventoryRepo;
        private readonly IMedicineRepo _medicineRepo;
        private readonly IPrescriptionRepo _prescriptionRepo;

        private const string CartSessionKey = "Cart";

        public CartController(
            IInventoryRepo inventoryRepo,
            IMedicineRepo medicineRepo,
            IPrescriptionRepo prescriptionRepo)
        {
            _inventoryRepo = inventoryRepo;
            _medicineRepo = medicineRepo;
            _prescriptionRepo = prescriptionRepo;
        }

        public IActionResult Index()
        {
            var cartItems = GetCart();
            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int medicineId)
        {
            var medicine = _medicineRepo.GetMedicineById(medicineId);

            if (medicine == null)
            {
                TempData["Error"] = "Medicine not found.";
                return RedirectToAction(nameof(Index));
            }

            if (medicine.RequiresPrescription)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                             ?? throw new InvalidOperationException("User is not authenticated.");

                var hasApprovedPrescription =
                    _prescriptionRepo.HasApprovedPrescriptionForMedicine(userId, medicineId);

                if (!hasApprovedPrescription)
                {
                    TempData["Error"] = "This medicine requires a verified prescription. Please upload one before purchasing.";
                    return RedirectToAction("Upload", "Prescription", new { medicineId });
                }
            }

            var inventory = _inventoryRepo.GetByMedicineId(medicineId);

            if (inventory == null || inventory.StockQuantity <= 0)
            {
                TempData["Error"] = "This medicine is currently out of stock.";
                return RedirectToAction(nameof(Index));
            }

            var cartItems = GetCart();
            var existingItem = cartItems.FirstOrDefault(c => c.MedicineId == medicineId);

            if (existingItem != null)
            {
                if (existingItem.Quantity >= inventory.StockQuantity)
                {
                    TempData["Error"] = "You cannot add more than the available stock.";
                    return RedirectToAction(nameof(Index));
                }

                existingItem.Quantity++;
            }
            else
            {
                cartItems.Add(new CartItem
                {
                    MedicineId = medicine.MedicineId,
                    MedicineName = medicine.MedicineName,
                    UnitPrice = medicine.Price,
                    Quantity = 1
                });
            }

            SaveCart(cartItems);

            TempData["Success"] = $"{medicine.MedicineName} added to cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateCartItem(int medicineId, int quantity)
        {
            var cartItems = GetCart();
            var item = cartItems.FirstOrDefault(c => c.MedicineId == medicineId);

            if (item == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var inventory = _inventoryRepo.GetByMedicineId(medicineId);

            if (quantity <= 0)
            {
                cartItems.Remove(item);
            }
            else if (inventory != null && quantity > inventory.StockQuantity)
            {
                TempData["Error"] = "Maximum available stock reached.";
            }
            else
            {
                item.Quantity = quantity;
            }

            SaveCart(cartItems);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveFromCart(int medicineId)
        {
            var cartItems = GetCart();
            var item = cartItems.FirstOrDefault(c => c.MedicineId == medicineId);

            if (item != null)
            {
                cartItems.Remove(item);
            }

            SaveCart(cartItems);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearCart()
        {
            HttpContext.Session.Remove(CartSessionKey);
            return RedirectToAction(nameof(Index));
        }

        private List<CartItem> GetCart()
        {
            return HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                   ?? new List<CartItem>();
        }

        private void SaveCart(List<CartItem> cartItems)
        {
            HttpContext.Session.SetObject(CartSessionKey, cartItems);
        }
    }
}