using EMEDS_Project.Data;
using EMEDS_Project.Models;
using EMEDS_Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepo _orderRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(
            IOrderRepo orderRepo,
            UserManager<ApplicationUser> userManager)
        {
            _orderRepo = orderRepo;
            _userManager = userManager;
        }


        // CUSTOMER - VIEW OWN ORDERS
        [Authorize(Roles = "Customer")]
        public IActionResult Index()
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            var orderList = _orderRepo.GetOrdersByUserId(userId);

            return View(orderList);
        }


        // CUSTOMER - VIEW OWN ORDERS
        [Authorize(Roles = "Customer")]
        public IActionResult MyOrders()
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            var orderList = _orderRepo.GetOrdersByUserId(userId);

            return View(orderList);
        }


        // CUSTOMER - ORDER DETAILS
        [Authorize(Roles = "Customer")]
        public IActionResult Details(int id)
        {
            var order = _orderRepo.GetOrderWithItems(id);

            if (order == null)
            {
                return NotFound();
            }

            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            // Customer cannot view another customer's order
            if (order.UserId != userId)
            {
                return Forbid();
            }

            return View(order);
        }


        // CREATE ORDER
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public IActionResult CreateOrder(Order order)
        {
            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            order.UserId = userId;
            order.OrderDate = DateTime.Now;
            order.OrderStatus = "Pending";

            // Calculate subtotal for each order item
            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Subtotal = orderItem.CalculateSubtotal();
            }

            // Calculate complete order total
            order.TotalAmount =
                order.OrderItems.Sum(orderItem => orderItem.Subtotal);

            int result = _orderRepo.AddOrder(order);

            if (result > 0)
            {
                TempData["Success"] =
                    "Order placed successfully.";

                return RedirectToAction(nameof(MyOrders));
            }

            TempData["Error"] =
                "Order could not be placed.";

            return RedirectToAction(nameof(MyOrders));
        }


        // CUSTOMER - CANCEL ORDER
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public IActionResult CancelOrder(int orderId)
        {
            var order = _orderRepo.GetOrderById(orderId);

            if (order == null)
            {
                return NotFound();
            }

            string? userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Unauthorized();
            }

            if (order.UserId != userId)
            {
                return Forbid();
            }

            // Delivered or already cancelled orders cannot be cancelled
            if (order.OrderStatus == "Delivered" ||
                order.OrderStatus == "Cancelled")
            {
                TempData["Error"] =
                    "This order cannot be cancelled.";

                return RedirectToAction(nameof(MyOrders));
            }

            _orderRepo.UpdateOrderStatus(
                orderId,
                "Cancelled");

            TempData["Success"] =
                "Order cancelled successfully.";

            return RedirectToAction(nameof(MyOrders));
        }


        // ADMIN - VIEW ALL ORDERS
        [Authorize(Roles = "Admin")]
        public IActionResult ManageOrders()
        {
            var orderList = _orderRepo.GetAllOrders();

            return View(orderList);
        }


        // ADMIN - ORDER DETAILS
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDetails(int id)
        {
            var order = _orderRepo.GetOrderWithItems(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // ADMIN - UPDATE ORDER STATUS
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateOrderStatus(
            int orderId,
            string orderStatus)
        {
            string[] allowedStatuses =
            {
                "Pending",
                "Confirmed",
                "Packed",
                "Shipped",
                "OutForDelivery",
                "Delivered",
                "Cancelled"
            };

            if (!allowedStatuses.Contains(orderStatus))
            {
                TempData["Error"] =
                    "Invalid order status.";

                return RedirectToAction(
                    nameof(AdminDetails),
                    new { id = orderId });
            }

            int result =
                _orderRepo.UpdateOrderStatus(
                    orderId,
                    orderStatus);

            if (result == 0)
            {
                return NotFound();
            }

            TempData["Success"] =
                "Order status updated successfully.";

            return RedirectToAction(
                nameof(AdminDetails),
                new { id = orderId });
        }
    }
}