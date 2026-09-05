using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
