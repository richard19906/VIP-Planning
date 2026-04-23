using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VIP_Planning.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home");
            }
            TempData["Error"] = "Ongeldige toegangscode";
            return View();
        }

        // DE BEVEILIGDE ADMIN BYPASS
        public IActionResult AdminBypass(string pin)
        {
            if (pin == "3991")
            {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home");
            }
            return Content("Toegang geweigerd: Onjuiste pincode.");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}