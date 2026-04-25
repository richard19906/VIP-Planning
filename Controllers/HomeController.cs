using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using VIP_Planning.Models;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        private readonly Supabase.Client _supabase;
        public HomeController(Supabase.Client supabase) { _supabase = supabase; }

        public IActionResult Index() => View();
        public IActionResult Instellingen() => View();

        public async Task<IActionResult> Planning() {
            var profielen = await _supabase.From<ProfielModel>().Get();
            ViewBag.Werknemers = profielen.Models ?? new List<ProfielModel>();
            var conceptJson = HttpContext.Session.GetString("ConceptRooster");
            var conceptLijst = string.IsNullOrEmpty(conceptJson) ? new List<UrenModel>() : JsonConvert.DeserializeObject<List<UrenModel>>(conceptJson);
            return View(conceptLijst);
        }

        // DEZE NAAM MOET EXACT 'Werknemers' ZIJN
        public async Task<IActionResult> Werknemers() {
            var response = await _supabase.From<ProfielModel>().Get();
            return View(response.Models ?? new List<ProfielModel>());
        }

        public async Task<IActionResult> UrenOverzicht(string email, string naam, string maand) {
            if (string.IsNullOrEmpty(maand)) {
                DateTime nu = DateTime.Now;
                maand = nu.Day >= 21 ? nu.AddMonths(1).ToString("MMM") : nu.ToString("MMM");
            }
            ViewBag.Email = email; ViewBag.Naam = naam; ViewBag.GeselecteerdeMaand = maand;
            ViewBag.Maanden = new List<string> { "Jan", "Feb", "Mrt", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec" };

            var response = await _supabase.From<UrenModel>().Where(x => x.UserEmail == email).Get();
            var gefilterd = (response.Models ?? new List<UrenModel>()).Where(x => x.PeriodeNaam == maand).ToList();
            return View(gefilterd.OrderBy(x => x.DatumString).ToList());
        }

        [HttpPost]
        public IActionResult OpslaanLokaal(UrenModel nieuw) {
            var conceptJson = HttpContext.Session.GetString("ConceptRooster");
            var lijst = string.IsNullOrEmpty(conceptJson) ? new List<UrenModel>() : JsonConvert.DeserializeObject<List<UrenModel>>(conceptJson);
            DateTime d = DateTime.Parse(nieuw.DatumString);
            nieuw.DatumString = d.ToString("dd-MM-yyyy");
            nieuw.PeriodeNaam = d.Day >= 21 ? d.AddMonths(1).ToString("MMM") : d.ToString("MMM");
            lijst.Add(nieuw);
            HttpContext.Session.SetString("ConceptRooster", JsonConvert.SerializeObject(lijst));
            return RedirectToAction("Planning");
        }

        [HttpPost]
        public async Task<IActionResult> PushNaarDatabase() {
            var conceptJson = HttpContext.Session.GetString("ConceptRooster");
            if (!string.IsNullOrEmpty(conceptJson)) {
                var lijst = JsonConvert.DeserializeObject<List<UrenModel>>(conceptJson);
                foreach(var item in lijst) { await _supabase.From<UrenModel>().Insert(item); }
                HttpContext.Session.Remove("ConceptRooster");
            }
            return RedirectToAction("Planning");
        }
    }
}