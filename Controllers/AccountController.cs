using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

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
            return RedirectToAction("VerifyCode");
        }

        [HttpPost]
        public IActionResult Verify(string username, string pincode) {
            string[] adminCodes = { "3991", "0000", "1234", "admin" };
            if (adminCodes.Contains(pincode)) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Toegang geweigerd.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}