using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        [HttpGet] public IActionResult Login() => View();
        [HttpGet] public IActionResult Register() => View();
        [HttpGet] public IActionResult VerifyCode() => View();
        [HttpGet] public IActionResult ForgotPassword() => View();

        [HttpPost]
        public IActionResult Register(string naam, string email, string password) {
            // Hier komt je Supabase SignUp logica. 
            // Voor nu sturen we door naar de verificatiepagina.
            TempData["UserEmail"] = email;
            return RedirectToAction("VerifyCode");
        }

        [HttpPost]
        public IActionResult VerifyCode(string code) {
            // Controleer hier de 6-cijferige code. 
            // Bij succes -> Login
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Verify(string username, string pincode) {
            string[] adminPincodes = { "3991", "0000", "1234", "admin" };
            if (adminPincodes.Contains(pincode) || (username == "admin" && pincode == "admin")) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", "Admin");
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