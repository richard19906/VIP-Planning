using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;
using VIP_Planning.Models;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        private readonly Supabase.Client _supabase;
        public HomeController(Supabase.Client supabase) { _supabase = supabase; }

        public IActionResult Index() => View();

        public async Task<IActionResult> Planning() {
            var response = await _supabase.From<PlanningModel>().Get();
            return View(response.Models);
        }

        public async Task<IActionResult> Werknemers() {
            var response = await _supabase.From<ProfielModel>().Get();
            return View(response.Models);
        }

        public IActionResult UrenOverzicht(string naam) {
            ViewBag.Naam = naam;
            return View();
        }

        public IActionResult Instellingen() => View();

        [HttpPost]
        public async Task<IActionResult> SavePlanning(string medewerker, string locatie, string datum, string uren) {
            var model = new PlanningModel { 
                Medewerker = medewerker, 
                Locatie = locatie, 
                Datum = datum, 
                Uren = uren 
            };
            await _supabase.From<PlanningModel>().Insert(model);
            return RedirectToAction("Planning");
        }
    }
}