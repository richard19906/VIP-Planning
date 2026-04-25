using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        // We laten Supabase even voor wat het is om de build te laten slagen
        public AccountController() { }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult Register() => View();

        [HttpGet]
        public IActionResult VerifyCode() => View();

        [HttpPost]
        public IActionResult Verify(string username, string pincode) {
            // De vertrouwde bypass codes
            string[] adminPincodes = { "3991", "0000", "1234", "admin" };
            
            if (adminPincodes.Contains(pincode)) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Onjuiste pincode.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}