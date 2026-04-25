using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        public AccountController() { }

        [HttpGet] public IActionResult Login() => View();
        [HttpGet] public IActionResult Register() => View();
        [HttpGet] public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult Register(string naam, string email, string password) {
            // Tijdelijke succes-redirect om 405 te voorkomen
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email) {
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Verify(string username, string pincode) {
            string[] adminPincodes = { "3991", "0000", "1234", "admin" };
            if (adminPincodes.Contains(pincode)) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Onjuiste gegevens.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}