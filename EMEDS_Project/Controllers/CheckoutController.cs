using System.Security.Claims;
using EMEDS_Project.Helpers;
using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using EMEDS_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

namespace EMEDS_Project.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly IOrderRepo _orderRepo;
        private readonly IPaymentRepo _paymentRepo;
        private const string CartSessionKey = "Cart";


        private readonly IMapper _mapper;

        public CheckoutController(
            IOrderRepo orderRepo,
            IPaymentRepo paymentRepo,
            IMapper mapper)
        {
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            var cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                            ?? new List<CartItem>();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = cartItems.Sum(c => c.TotalPrice)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(CheckoutViewModel model)
        {
            var cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                            ?? new List<CartItem>();

            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";
                return RedirectToAction("Index", "Cart");
            }

            if (!ModelState.IsValid)
            {
                model.CartItems = cartItems;
                model.TotalAmount = cartItems.Sum(c => c.TotalPrice);
                return View("Index", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? throw new InvalidOperationException("User is not authenticated.");

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.Now,
                TotalAmount = cartItems.Sum(c => c.TotalPrice),
                DeliveryAddress = $"{model.Address}, {model.City}, {model.PostalCode}",
                OrderStatus = "Pending",
                OrderItems = _mapper.Map<List<OrderItem>>(cartItems)
            };

            _orderRepo.AddOrder(order);

            var payment = new Payment
            {
                OrderId = order.OrderId,
                Amount = order.TotalAmount,
                PaymentMethod = model.PaymentMethod,
                PaymentStatus = model.PaymentMethod == "Cash on Delivery" ? "Pending" : "Paid",
                PaymentDate = DateTime.Now
            };

            _paymentRepo.Add(payment);
            _paymentRepo.Save();

            HttpContext.Session.Remove(CartSessionKey);

            TempData["Success"] = "Your order has been placed successfully.";
            return RedirectToAction(nameof(Confirmation), new { orderId = order.OrderId });
        }

        public IActionResult Confirmation(int orderId)
        {
            var order = _orderRepo.GetOrderWithItems(orderId);

            if (order == null)
            {
                return NotFound();
            }

            var payment = _paymentRepo.GetByOrderId(orderId);
            ViewBag.PaymentStatus = payment?.PaymentStatus ?? "Unknown";

            return View(order);
        }
    }
}