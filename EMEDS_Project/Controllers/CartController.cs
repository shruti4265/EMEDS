using EMEDS_Project.Helpers;
using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    public class CartController : Controller
    {
        private readonly IInventoryRepo _inventoryRepo;

        private const string CartSessionKey = "Cart";

        public CartController(IInventoryRepo inventoryRepo)
        {
            _inventoryRepo = inventoryRepo;
        }

        public IActionResult Index()
        {
            var cartItems = GetCart();
            return View(cartItems);
        }

        // TEMP: takes medicineName/unitPrice directly until Chhaya's Medicine
        // module exists. Once it does, this collapses down to just (int medicineId)
        // and looks the name/price up via IMedicineRepo.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddToCart(int medicineId, string medicineName, decimal unitPrice)
        {
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
                    MedicineId = medicineId,
                    MedicineName = medicineName,
                    UnitPrice = unitPrice,
                    Quantity = 1
                });
            }

            SaveCart(cartItems);

            TempData["Success"] = $"{medicineName} added to cart.";
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