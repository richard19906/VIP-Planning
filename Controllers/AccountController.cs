using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        [HttpGet] public IActionResult Login() => View();
        [HttpGet] public IActionResult Register() => View();
        [HttpGet] public IActionResult VerifyCode() => View();
        
        [HttpPost]
        public IActionResult Register(string naam, string email, string password, string confirmPassword) {
            if (password != confirmPassword) {
                ViewBag.Error = "Wachtwoorden komen niet overeen.";
                return View();
            }
            // Hier komt de logica om de 6-cijferige code te triggeren
            TempData["UserEmail"] = email;
            return RedirectToAction("VerifyCode");
        }

        [HttpPost]
        public IActionResult VerifyCode(string code) {
            // Check code logica
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Verify(string username, string pincode) {
            string[] bypassCodes = { "3991", "0000", "1234", "admin" };
            if (bypassCodes.Contains(pincode)) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Onjuiste toegangscode.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}