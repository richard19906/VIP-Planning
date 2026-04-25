using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Verify(string username, string pincode) {
            // De lijst met speciale testcodes/admin codes
            string[] adminPincodes = { "3991", "0000", "1234", "admin" };

            // Check of de pincode een admin/bypass code is
            if (adminPincodes.Contains(pincode)) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("Index", "Home");
            }

            // Check voor klassieke admin combinatie
            if (username == "admin" && pincode == "admin") {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("Index", "Home");
            }

            // Als inloggen mislukt
            ViewBag.Error = "Toegang geweigerd: Onjuiste code of gebruiker.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}