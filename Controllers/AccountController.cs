using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using System.Threading.Tasks;

namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        private readonly Supabase.Client _supabase;
        public AccountController(Supabase.Client supabase) { _supabase = supabase; }

        [HttpGet] public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string naam, string email, string password) {
            try {
                var options = new Supabase.Gotrue.SignUpOptions {
                    Data = new System.Collections.Generic.Dictionary<string, object> { { "full_name", naam } }
                };
                await _supabase.Auth.SignUp(email, password, options);
                
                // Na registratie sturen we ze naar het code-invulscherm
                TempData["Email"] = email;
                return RedirectToAction("VerifyCode");
            } catch (System.Exception ex) {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpGet] public IActionResult VerifyCode() => View();

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string email, string code) {
            try {
                // Verifieer de 6-cijferige code bij Supabase
                var session = await _supabase.Auth.VerifyOTP(email, code, Supabase.Gotrue.Constants.EmailOtpType.Signup);
                if (session != null) {
                    ViewBag.Message = "Account geverifieerd! Je kunt nu inloggen.";
                    return View("Login");
                }
            } catch (System.Exception ex) {
                ViewBag.Error = "Ongeldige code: " + ex.Message;
            }
            return View();
        }
    }
}