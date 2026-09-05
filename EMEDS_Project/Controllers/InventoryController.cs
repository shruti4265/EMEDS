using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
