using EMEDS_Project.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    [Authorize]
    public class ChatbotController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            ChatbotViewModel model = new ChatbotViewModel();

            model.Questions = GetQuestions();

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(string question)
        {
            ChatbotViewModel model = new ChatbotViewModel();

            model.Questions = GetQuestions();
            model.Question = question;

            if (question == "How do I search for a medicine?")
            {
                model.Answer =
                    "Go to the Medicines section where you can browse and search for available medicines.";
            }
            else if (question == "How do I add a medicine to my cart?")
            {
                model.Answer =
                    "Open the Medicines section, select the required medicine and use the Add to Cart option.";
            }
            else if (question == "How can I upload a prescription?")
            {
                model.Answer =
                    "Go to the Prescription section and click Upload Prescription. Select the required prescription file and submit it.";
            }
            else if (question == "Which prescription file formats are allowed?")
            {
                model.Answer =
                    "You can upload prescription files in JPG, JPEG, PNG or PDF format.";
            }
            else if (question == "What is the maximum prescription file size?")
            {
                model.Answer =
                    "The maximum allowed prescription file size is 5 MB.";
            }
            else if (question == "How can I place an order?")
            {
                model.Answer =
                    "Add the required medicines to your cart, proceed to checkout, enter the required details and complete the order.";
            }
            else if (question == "How can I view my orders?")
            {
                model.Answer =
                    "Go to the My Orders section to view your current and previous orders.";
            }
            else if (question == "How can I cancel my order?")
            {
                model.Answer =
                    "Go to My Orders, open the required order and select Cancel Order if the order is eligible for cancellation.";
            }
            else if (question == "How can I track my order?")
            {
                model.Answer =
                    "Open My Orders and select Order Details. The current Order Status shows the progress of your order.";
            }
            else if (question == "What are the different order statuses?")
            {
                model.Answer =
                    "The order statuses are Pending, Confirmed, Packed, Shipped, Out For Delivery, Delivered and Cancelled.";
            }
            else if (question == "How does payment work?")
            {
                model.Answer =
                    "Payment is handled during the checkout process after confirming the medicines and order details.";
            }
            else
            {
                model.Answer =
                    "Sorry, I could not find an answer for that question.";
            }

            return View(model);
        }

        private List<string> GetQuestions()
        {
            return new List<string>
            {
                "How do I search for a medicine?",
                "How do I add a medicine to my cart?",
                "How can I upload a prescription?",
                "Which prescription file formats are allowed?",
                "What is the maximum prescription file size?",
                "How can I place an order?",
                "How can I view my orders?",
                "How can I cancel my order?",
                "How can I track my order?",
                "What are the different order statuses?",
                "How does payment work?"
            };
        }
    }
}