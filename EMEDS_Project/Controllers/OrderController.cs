using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
