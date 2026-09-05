using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
