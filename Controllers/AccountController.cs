using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        [HttpGet] public IActionResult Login() => View();
        
        [HttpPost]
        public IActionResult Login(string username) {
            if (!string.IsNullOrEmpty(username)) {
                TempData["Username"] = username;
                return RedirectToAction("VerifyCode");
            }
            return View();
        }

        [HttpGet] public IActionResult VerifyCode() {
            ViewBag.Username = TempData["Username"];
            return View();
        }

        [HttpPost]
        public IActionResult Verify(string pincode) {
            // De vertrouwde bypass codes
            if (pincode == "3991" || pincode == "0000") {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}