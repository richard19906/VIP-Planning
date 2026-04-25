using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() => View();
        public IActionResult Planning() => View();
        public IActionResult Instellingen() => View();

        [HttpPost]
        public IActionResult SavePincode(string newPin) {
            return RedirectToAction("Instellingen");
        }

        [HttpPost]
        public IActionResult UpdatePassword(string oldPw, string newPw) {
            return RedirectToAction("Instellingen");
        }
    }

    public class AccountController : Controller {
        [HttpGet] public IActionResult Login() => View();
        [HttpGet] public IActionResult Register() => View();
        [HttpGet] public IActionResult VerifyCode() => View();

        [HttpPost]
        public IActionResult Verify(string username, string pincode) {
            if (pincode == "3991" || pincode == "0000") {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                return RedirectToAction("Index", "Home");
            }
            return View("Login");
        }
        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}