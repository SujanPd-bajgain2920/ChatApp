using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    public class StaticController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
