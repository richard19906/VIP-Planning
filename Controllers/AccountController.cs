using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using System.Threading.Tasks;
using System.Linq;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        private readonly Supabase.Client _supabase;

        public AccountController(Supabase.Client supabase) {
            _supabase = supabase;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password, string confirmPassword) {
            if (password != confirmPassword) {
                ViewBag.Error = "Wachtwoorden komen niet overeen.";
                return View();
            }

            try {
                // Dit maakt de gebruiker aan in Supabase Auth
                // Supabase stuurt daarna de mail via jouw ingestelde SMTP
                var response = await _supabase.Auth.SignUp(email, password);
                
                if (response != null) {
                    ViewBag.Message = "Succes! Check je mail om je account te bevestigen.";
                    return View("Login");
                }
            } catch (System.Exception ex) {
                ViewBag.Error = "Registratie mislukt: " + ex.Message;
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email) {
            try {
                // Verstuurt de 'Reset Password' mail via Supabase
                await _supabase.Auth.ResetPasswordForEmail(email);
                ViewBag.Message = "Er is een herstel-mail gestuurd naar " + email;
            } catch (System.Exception ex) {
                ViewBag.Error = "Fout: " + ex.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Verify(string username, string pincode) {
            // Snelle admin-bypass blijft bestaan
            string[] adminPincodes = { "3991", "0000", "1234", "admin" };
            if (adminPincodes.Contains(pincode)) {
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserRole", "Admin");
                return RedirectToAction("Index", "Home");
            }

            // Normale login check via Supabase Auth kan hier worden toegevoegd
            ViewBag.Error = "Onjuiste gegevens of account niet bevestigd.";
            return View("Login");
        }

        public IActionResult Logout() {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}