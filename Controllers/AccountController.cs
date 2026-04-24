using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        [HttpGet] public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string password) {
            if (password == "VIP2024") {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home"); // FORCEER DASHBOARD
            }
            ViewBag.Error = "Onjuist wachtwoord";
            return View();
        }

        [HttpGet]
        public IActionResult AdminBypass(string pin) {
            if (pin == "3991") {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home"); // FORCEER DASHBOARD
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}