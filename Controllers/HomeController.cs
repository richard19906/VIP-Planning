using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VIP_Planning.Controllers
{
    public class HomeController : Controller
    {
        // Deze functie kijkt of de sessie bestaat
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsLoggedIn")))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }
    }
}