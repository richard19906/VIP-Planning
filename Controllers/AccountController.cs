using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using System.Threading.Tasks;
using System.Linq;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        private readonly Supabase.Client _supabase;
        public AccountController(Supabase.Client supabase) { _supabase = supabase; }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Verify(string username, string pincode) {
            string[] adminPincodes = { "3991", "0000", "1234", "admin" };
            if (adminPincodes.Contains(pincode)) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Error = "Inloggen mislukt. Gebruik een geldige code.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet] public IActionResult Register() => View();
        [HttpGet] public IActionResult VerifyCode() => View();
    }
}