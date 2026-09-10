//using System.Security.Claims;
//using EMEDS_Project.Helpers;
//using EMEDS_Project.Models;
//using EMEDS_Project.Repository;
//using EMEDS_Project.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using AutoMapper;

//namespace EMEDS_Project.Controllers
//{
//    [Authorize]
//    public class CheckoutController : Controller
//    {
//        private readonly IOrderRepo _orderRepo;
//        private readonly IPaymentRepo _paymentRepo;
//        private const string CartSessionKey = "Cart";


//        private readonly IMapper _mapper;

//        public CheckoutController(
//            IOrderRepo orderRepo,
//            IPaymentRepo paymentRepo,
//            IMapper mapper)
//        {
//            _orderRepo = orderRepo;
//            _paymentRepo = paymentRepo;
//            _mapper = mapper;
//        }

//        public IActionResult Index()
//        {
//            var cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
//                            ?? new List<CartItem>();

//            if (!cartItems.Any())
//            {
//                TempData["Error"] = "Your cart is empty.";
//                return RedirectToAction("Index", "Cart");
//            }

//            var model = new CheckoutViewModel
//            {
//                CartItems = cartItems,
//                TotalAmount = cartItems.Sum(c => c.TotalPrice)
//            };

//            return View(model);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult PlaceOrder(CheckoutViewModel model)
//        {
//            var cartItems = HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
//                            ?? new List<CartItem>();

//            if (!cartItems.Any())
//            {
//                TempData["Error"] = "Your cart is empty.";
//                return RedirectToAction("Index", "Cart");
//            }

//            if (!ModelState.IsValid)
//            {
//                model.CartItems = cartItems;
//                model.TotalAmount = cartItems.Sum(c => c.TotalPrice);
//                return View("Index", model);
//            }

//            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
//                         ?? throw new InvalidOperationException("User is not authenticated.");

//            var order = new Order
//            {
//                UserId = userId,
//                OrderDate = DateTime.Now,
//                TotalAmount = cartItems.Sum(c => c.TotalPrice),
//                DeliveryAddress = $"{model.Address}, {model.City}, {model.PostalCode}",
//                OrderStatus = "Pending",
//                OrderItems = _mapper.Map<List<OrderItem>>(cartItems)
//            };

//            _orderRepo.AddOrder(order);

//            var payment = new Payment
//            {
//                OrderId = order.OrderId,
//                Amount = order.TotalAmount,
//                PaymentMethod = model.PaymentMethod,
//                PaymentStatus = model.PaymentMethod == "Cash on Delivery" ? "Pending" : "Paid",
//                PaymentDate = DateTime.Now
//            };

//            _paymentRepo.Add(payment);
//            _paymentRepo.Save();

//            HttpContext.Session.Remove(CartSessionKey);

//            TempData["Success"] = "Your order has been placed successfully.";
//            return RedirectToAction(nameof(Confirmation), new { orderId = order.OrderId });
//        }

//        public IActionResult Confirmation(int orderId)
//        {
//            var order = _orderRepo.GetOrderWithItems(orderId);

//            if (order == null)
//            {
//                return NotFound();
//            }

//            var payment = _paymentRepo.GetByOrderId(orderId);
//            ViewBag.PaymentStatus = payment?.PaymentStatus ?? "Unknown";

//            return View(order);
//        }
//    }
//}








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
        private readonly IPrescriptionRepo _prescriptionRepo;
        private readonly IMedicineRepo _medicineRepo;
        private readonly IWebHostEnvironment _environment;
        private readonly IMapper _mapper;

        private const string CartSessionKey = "Cart";


        public CheckoutController(
            IOrderRepo orderRepo,
            IPaymentRepo paymentRepo,
            IPrescriptionRepo prescriptionRepo,
            IMedicineRepo medicineRepo,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _orderRepo = orderRepo;
            _paymentRepo = paymentRepo;
            _prescriptionRepo = prescriptionRepo;
            _medicineRepo = medicineRepo;
            _mapper = mapper;
            _environment = environment;
        }


        public IActionResult Index()
        {
            var cartItems =
                HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
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
        public IActionResult PlaceOrder(
            CheckoutViewModel model,
            IFormFile? prescriptionFile)
        {
            var cartItems =
                HttpContext.Session.GetObject<List<CartItem>>(CartSessionKey)
                ?? new List<CartItem>();


            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty.";

                return RedirectToAction(
                    "Index",
                    "Cart");
            }


            // Find medicines in cart that require prescription

            var prescriptionItems = cartItems
                .Where(cartItem =>
                {
                    var medicine =
                        _medicineRepo.GetMedicineById(
                            cartItem.MedicineId);

                    return medicine != null &&
                           medicine.RequiresPrescription;
                })
                .ToList();


            bool requiresPrescription =
                prescriptionItems.Any();


            // If Rx medicine exists,
            // prescription must be uploaded at checkout

            if (requiresPrescription)
            {
                if (prescriptionFile == null ||
                    prescriptionFile.Length == 0)
                {
                    ModelState.AddModelError(
                        "",
                        "Please upload a prescription before placing the order.");
                }
                else
                {
                    string extension =
                        Path.GetExtension(
                            prescriptionFile.FileName)
                        .ToLowerInvariant();


                    string[] allowedExtensions =
                    {
                        ".jpg",
                        ".jpeg",
                        ".png",
                        ".pdf"
                    };


                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError(
                            "",
                            "Only JPG, JPEG, PNG and PDF files are allowed.");
                    }


                    if (prescriptionFile.Length >
                        5 * 1024 * 1024)
                    {
                        ModelState.AddModelError(
                            "",
                            "Prescription file cannot exceed 5 MB.");
                    }
                }
            }


            if (!ModelState.IsValid)
            {
                model.CartItems = cartItems;

                model.TotalAmount =
                    cartItems.Sum(c => c.TotalPrice);

                return View(
                    "Index",
                    model);
            }


            var userId =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier)
                ?? throw new InvalidOperationException(
                    "User is not authenticated.");


            // Create order

            var order = new Order
            {
                UserId = userId,

                OrderDate = DateTime.Now,

                TotalAmount =
                    cartItems.Sum(
                        c => c.TotalPrice),

                DeliveryAddress =
                    $"{model.Address}, {model.City}, {model.PostalCode}",

                // Order becomes confirmed after
                // prescription is uploaded at checkout

                OrderStatus = "Confirmed",

                OrderItems =
                    _mapper.Map<List<OrderItem>>(
                        cartItems)
            };


            _orderRepo.AddOrder(order);


            // Save prescription and link it
            // with this order

            if (requiresPrescription &&
                prescriptionFile != null)
            {
                string extension =
                    Path.GetExtension(
                        prescriptionFile.FileName)
                    .ToLowerInvariant();


                string uploadFolder =
                    Path.Combine(
                        _environment.WebRootPath,
                        "prescriptions");


                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(
                        uploadFolder);
                }


                string uniqueFileName =
                    Guid.NewGuid().ToString()
                    + extension;


                string physicalPath =
                    Path.Combine(
                        uploadFolder,
                        uniqueFileName);


                using (var stream =
                    new FileStream(
                        physicalPath,
                        FileMode.Create))
                {
                    prescriptionFile.CopyTo(
                        stream);
                }


                // Link prescription with every
                // Rx medicine present in the order

                foreach (var item in prescriptionItems)
                {
                    var prescription =
                        new Prescription
                        {
                            UserId = userId,

                            MedicineId =
                                item.MedicineId,

                            OrderId =
                                order.OrderId,

                            FilePath =
                                "/prescriptions/" +
                                uniqueFileName,

                            UploadDate =
                                DateTime.Now,

                            Status =
                                "Pending"
                        };


                    _prescriptionRepo
                        .AddPrescription(
                            prescription);
                }
            }


            // Payment

            var payment = new Payment
            {
                OrderId =
                    order.OrderId,

                Amount =
                    order.TotalAmount,

                PaymentMethod =
                    model.PaymentMethod,

                PaymentStatus =
                    model.PaymentMethod ==
                    "Cash on Delivery"
                        ? "Pending"
                        : "Paid",

                PaymentDate =
                    DateTime.Now
            };


            _paymentRepo.Add(payment);
            _paymentRepo.Save();


            // Clear cart

            HttpContext.Session.Remove(
                CartSessionKey);


            TempData["Success"] =
                "Your order has been placed successfully.";


            return RedirectToAction(
                nameof(Confirmation),
                new
                {
                    orderId = order.OrderId
                });
        }


        public IActionResult Confirmation(
            int orderId)
        {
            var order =
                _orderRepo.GetOrderWithItems(
                    orderId);


            if (order == null)
            {
                return NotFound();
            }


            var payment =
                _paymentRepo.GetByOrderId(
                    orderId);


            ViewBag.PaymentStatus =
                payment?.PaymentStatus
                ?? "Unknown";


            return View(order);
        }
    }
}