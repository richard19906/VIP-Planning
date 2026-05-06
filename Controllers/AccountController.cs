using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using System;
using System.Threading.Tasks;
using VIP_Planning.Models;

namespace VIP_Planning.Controllers
{
    public class AccountController : Controller
    {
        private readonly Supabase.Client _supabase;

        public AccountController(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public IActionResult Register() => View();

        // --- WACHTWOORD VERGETEN ---
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                await _supabase.Auth.ResetPasswordForEmail(email);
                ViewBag.Message = "Als dit e-mailadres bekend is, ontvangt u een herstelmail.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Er is een fout opgetreden: " + ex.Message;
                return View();
            }
        }

        // --- REGISTRATIE ---
        [HttpPost]
        public async Task<IActionResult> Register(string naam, string email, string password)
        {
            try
            {
                await _supabase.Auth.SignUp(email, password);

                var nieuwProfiel = new ProfielModel
                {
                    Email = email,
                    Naam = naam,
                    Rol = "Werkgever",
                    AanmaakDatum = DateTime.Now.ToString("dd-MM-yyyy")
                };
                await _supabase.From<ProfielModel>().Insert(nieuwProfiel);

                TempData["Email"] = email;
                return RedirectToAction("VerifyCode");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Fout bij registreren: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult VerifyCode()
        {
            ViewBag.Email = TempData["Email"];
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyCode(string email, string code)
        {
            try
            {
                var session = await _supabase.Auth.VerifyOTP(email, code, Supabase.Gotrue.Constants.EmailOtpType.Signup);

                if (session != null)
                {
                    return RedirectToAction("Login", new { success = "geactiveerd" });
                }
                ViewBag.Error = "Code onjuist.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Error = "Verificatie mislukt.";
                return View();
            }
        }

        // --- INLOGGEN VERIFICATIE (Gefixt voor Dashboard toegang) ---
        [HttpPost]
        public async Task<IActionResult> Verify(string username, string pincode)
        {
            // Admin Bypass: Code 3991 of 0000
            if (username == "ADMIN_BYPASS" && (pincode == "3991" || pincode == "0000"))
            {
                // CRUCIAAL: Beide sessies vullen, anders springt Home/Index terug naar Login
                HttpContext.Session.SetString("UserEmail", "Admin");
                HttpContext.Session.SetString("UserRole", "Admin");

                return RedirectToAction("Index", "Home");
            }

            try
            {
                // Normale login via Supabase
                var session = await _supabase.Auth.SignIn(username, pincode);
                if (session != null)
                {
                    HttpContext.Session.SetString("UserEmail", username);
                    HttpContext.Session.SetString("UserRole", "Gebruiker");

                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Login");
            }
            catch
            {
                ViewBag.Error = "Inloggegevens onjuist.";
                return View("Login");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}