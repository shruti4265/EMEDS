using Microsoft.AspNetCore.Mvc;

namespace EMEDS_Project.Controllers
{
    public class PrescriptionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
