using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        [HttpGet] public IActionResult Login() => View();
        [HttpGet] public IActionResult Register() => View();
        [HttpGet] public IActionResult VerifyCode() => View();
        [HttpGet] public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult Register(string naam, string email, string password) {
            TempData["UserEmail"] = email;
            // Hier zou de Supabase SignUp komen die de 6-cijferige code triggert
            return RedirectToAction("VerifyCode");
        }

        [HttpPost]
        public IActionResult VerifyCode(string code) {
            // Logica voor checken 6-cijferige OTP code
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
            ViewBag.Error = "Toegang geweigerd. Onjuiste pincode.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}