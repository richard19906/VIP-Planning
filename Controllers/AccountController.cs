using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        [HttpGet] public IActionResult Login() => View();
        [HttpGet] public IActionResult Register() => View();
        [HttpGet] public IActionResult VerifyCode() => View();

        [HttpPost]
        public IActionResult Verify(string username, string pincode) {
            // De vertrouwde bypass codes
            string[] bypassCodes = { "3991", "0000", "1234", "admin" };
            if (bypassCodes.Contains(pincode)) {
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