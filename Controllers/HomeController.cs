using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using VIP_Planning.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        private readonly Supabase.Client _supabase;
        public HomeController(Supabase.Client supabase) { _supabase = supabase; }

        public async Task<IActionResult> Index() {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsLoggedIn"))) return RedirectToAction("Login", "Account");
            var werknemers = await _supabase.From<ProfielModel>().Get();
            var planning = await _supabase.From<PlanningModel>().Get();
            ViewBag.TotaalWerknemers = werknemers.Models?.Count ?? 0;
            ViewBag.OpenstaandeDiensten = planning.Models?.Where(x => !x.IsGepusht).Count() ?? 0;
            return View();
        }

        public async Task<IActionResult> Werknemers() {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsLoggedIn"))) return RedirectToAction("Login", "Account");
            var response = await _supabase.From<ProfielModel>().Get();
            return View(response.Models ?? new List<ProfielModel>());
        }

        public async Task<IActionResult> Planning() {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsLoggedIn"))) return RedirectToAction("Login", "Account");
            var response = await _supabase.From<PlanningModel>().Get();
            var lijst = (response.Models ?? new List<PlanningModel>()).OrderBy(p => p.Datum).ToList();
            return View(lijst);
        }

        public IActionResult Instellingen() {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("IsLoggedIn"))) return RedirectToAction("Login", "Account");
            return View();
        }
    }
}