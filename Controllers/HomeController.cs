using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using VIP_Planning.Models;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        private readonly Supabase.Client _supabase;
        public HomeController(Supabase.Client supabase) { _supabase = supabase; }

        // Het hoofdscherm (Dashboard) is nu algemeen
        public IActionResult Index() {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsLoggedIn"))) 
                return RedirectToAction("Login", "Account");
            return View();
        }

        // DEZE ACTIE TOONT PAS DE UREN
        public async Task<IActionResult> GewerkteUren() {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "Account");

            try {
                var response = await _supabase.From<UrenModel>()
                    .Filter("user_email", Postgrest.Constants.Operator.Equals, email)
                    .Get();
                return View(response.Models);
            } catch {
                return View(new List<UrenModel>());
            }
        }
    }
}