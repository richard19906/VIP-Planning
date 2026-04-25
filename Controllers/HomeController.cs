using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Threading.Tasks;
using System.Linq;
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

        public async Task<IActionResult> UrenOverzicht(string naam) {
            ViewBag.Naam = naam;
            // Haal uren op waarbij medewerker gelijk is aan de geklikte naam
            var response = await _supabase.From<PlanningModel>()
                .Where(x => x.Medewerker == naam)
                .Get();
            return View(response.Models);
        }

        public IActionResult Instellingen() => View();
    }
}