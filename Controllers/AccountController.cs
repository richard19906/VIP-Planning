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

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string naam, string email, string password) {
            try {
                var options = new Supabase.Gotrue.SignUpOptions {
                    Data = new System.Collections.Generic.Dictionary<string, object> { { "full_name", naam } }
                };
                await _supabase.Auth.SignUp(email, password, options);
                TempData["Email"] = email;
                return RedirectToAction("VerifyCode");
            } catch (System.Exception ex) {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult VerifyCode() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string email, string code) {
            try {
                var session = await _supabase.Auth.VerifyOTP(email, code, Supabase.Gotrue.Constants.EmailOtpType.Signup);
                if (session != null) {
                    return RedirectToAction("Login");
                }
            } catch (System.Exception ex) {
                ViewBag.Error = "Code onjuist: " + ex.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify(string username, string pincode) {
            string[] adminPincodes = { "3991", "0000", "1234", "admin" };
            if (adminPincodes.Contains(pincode)) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("Index", "Home");
            }
            // Hier kun je Supabase SignIn toevoegen voor gewone users
            ViewBag.Error = "Inloggen mislukt.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}