using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    public class SupplierController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
