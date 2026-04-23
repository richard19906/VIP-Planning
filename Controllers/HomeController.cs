using Microsoft.AspNetCore.Mvc;
using Supabase;
using VIP_Planning.Models;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        private readonly Supabase.Client _supabase;
        public HomeController(Supabase.Client supabase) { _supabase = supabase; }

        public async Task<IActionResult> Index() {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail)) return RedirectToAction("Login", "Account");

            try {
                // We filteren op de kolom 'user_email'
                var response = await _supabase.From<UrenModel>()
                    .Filter("user_email", Postgrest.Constants.Operator.Equals, userEmail)
                    .Get();
                
                ViewBag.UserName = HttpContext.Session.GetString("UserName");
                return View(response.Models);
            } catch (Exception ex) {
                ViewBag.Error = ex.Message;
                return View(new List<UrenModel>());
            }
        }
    }
}