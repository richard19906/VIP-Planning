using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using VIP_Planning.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        private readonly Supabase.Client _supabase;
        public HomeController(Supabase.Client supabase) { _supabase = supabase; }

        public IActionResult Index() {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsLoggedIn"))) 
                return RedirectToAction("Login", "Account");
            return View();
        }

        public async Task<IActionResult> GewerkteUren() {
            var email = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login", "Account");

            try {
                var response = await _supabase.From<UrenModel>()
                    .Filter("user_email", Postgrest.Constants.Operator.Equals, email)
                    .Get();
                
                var uren = response.Models ?? new List<UrenModel>();
                return View(uren);
            } catch (Exception ex) {
                // Als er iets misgaat, toon een lege lijst i.p.v. een Error 500
                Console.WriteLine(ex.Message);
                return View(new List<UrenModel>());
            }
        }
    }
}