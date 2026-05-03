using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using VIP_Planning.Models;

namespace VIP_Planning.Controllers
{
    public class HomeController : Controller
    {
        private readonly Supabase.Client _supabase;

        public HomeController(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public IActionResult Index() => View();
        public IActionResult Instellingen() => View();

        // Haalt live data uit de 'planning' tabel en ondersteunt de maandkiezer filtering
        public async Task<IActionResult> Planning(string maand)
        {
            var culture = new CultureInfo("nl-NL");

            // 1. Profielen ophalen voor de dropdown
            var profielen = await _supabase.From<ProfielModel>().Get();
            ViewBag.Werknemers = profielen.Models ?? new List<ProfielModel>();

            // 2. Bepaal de geselecteerde maand voor de maandkiezer-styling
            if (string.IsNullOrEmpty(maand))
            {
                maand = culture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMM", culture).Replace(".", ""));
            }
            ViewBag.GeselecteerdeMaand = maand;

            // 3. Live planning ophalen uit Supabase
            var response = await _supabase.From<PlanningModel>().Get();
            var planningLijst = response.Models ?? new List<PlanningModel>();

            // 4. FILTERING: Toon alleen diensten die vallen in de geselecteerde maand
            var gefilterdeLijst = planningLijst.Where(x =>
            {
                if (DateTime.TryParse(x.Datum, out DateTime d))
                {
                    return d.ToString("MMM", culture).Replace(".", "").Equals(maand, StringComparison.OrdinalIgnoreCase);
                }
                return false;
            }).OrderBy(x => x.Datum).ToList();

            return View(gefilterdeLijst);
        }

        public async Task<IActionResult> Werknemers()
        {
            var response = await _supabase.From<ProfielModel>().Get();
            return View(response.Models ?? new List<ProfielModel>());
        }

        public async Task<IActionResult> UrenOverzicht(string email, string naam, string maand)
        {
            var culture = new CultureInfo("nl-NL");
            if (string.IsNullOrEmpty(maand))
            {
                DateTime nu = DateTime.Now;
                DateTime targetDate = nu.Day >= 21 ? nu.AddMonths(1) : nu;
                maand = culture.TextInfo.ToTitleCase(targetDate.ToString("MMM", culture).Replace(".", ""));
            }

            ViewBag.Email = email;
            ViewBag.Naam = naam;
            ViewBag.GeselecteerdeMaand = maand;
            ViewBag.Maanden = new List<string> { "Jan", "Feb", "Mrt", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec" };

            var response = await _supabase.From<UrenModel>().Where(x => x.UserEmail == email).Get();
            var gefilterd = (response.Models ?? new List<UrenModel>())
                .Where(x => x.PeriodeNaam != null && x.PeriodeNaam.Equals(maand, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.DatumString)
                .ToList();

            return View(gefilterd);
        }

        [HttpPost]
        public async Task<IActionResult> OpslaanLokaal(UrenModel nieuw)
        {
            // 1. Zoek het profiel van de werknemer
            var profielenResponse = await _supabase.From<ProfielModel>().Get();
            var werknemerProfiel = profielenResponse.Models?.FirstOrDefault(p => p.Email == nieuw.UserEmail);

            if (werknemerProfiel == null) return RedirectToAction("Planning");

            // 2. Datum voorbereiden (database formaat)
            string dbDatum = "";
            if (DateTime.TryParse(nieuw.DatumString, out DateTime d))
            {
                dbDatum = d.ToString("yyyy-MM-dd");
            }
            else
            {
                return RedirectToAction("Planning");
            }

            // 3. --- SLIMMERE BEVEILIGING TEGEN DUBBELE INVOER ---
            // Sta 2 diensten op 1 dag toe, MAAR alleen als de locatie verschilt
            var bestaand = await _supabase.From<PlanningModel>()
                .Where(x => x.UserEmail == nieuw.UserEmail)
                .Where(x => x.Datum == dbDatum)
                .Where(x => x.Locatie == nieuw.Locatie)
                .Get();

            if (bestaand.Models.Any())
            {
                return RedirectToAction("Planning", new { fout = "dubbel" });
            }

            // 4. Nieuw item aanmaken
            var roosterItem = new PlanningModel
            {
                UserEmail = nieuw.UserEmail,
                UserNaam = werknemerProfiel.Naam,
                Locatie = nieuw.Locatie,
                Uren = nieuw.Uren,
                Datum = dbDatum,
                IsGepusht = false,
                StartTijd = "00:00",
                EindTijd = "00:00"
            };

            // 5. Direct invoegen in Supabase
            await _supabase.From<PlanningModel>().Insert(roosterItem);

            var culture = new CultureInfo("nl-NL");
            string maandVanDienst = DateTime.Parse(dbDatum).ToString("MMM", culture).Replace(".", "");
            return RedirectToAction("Planning", new { maand = culture.TextInfo.ToTitleCase(maandVanDienst) });
        }

        [HttpPost]
        public async Task<IActionResult> VerwijderPlanning(long id)
        {
            await _supabase.From<PlanningModel>().Where(x => x.Id == id).Delete();
            return RedirectToAction("Planning");
        }

        [HttpPost]
        public async Task<IActionResult> PushNaarDatabase()
        {
            var response = await _supabase.From<PlanningModel>().Where(x => x.IsGepusht == false).Get();
            var nietGepusht = response.Models;

            if (nietGepusht != null)
            {
                foreach (var item in nietGepusht)
                {
                    item.IsGepusht = true;
                    await _supabase.From<PlanningModel>().Update(item);
                }
            }

            return RedirectToAction("Planning");
        }

        [HttpPost]
        public IActionResult VerwijderUitConcept(int index)
        {
            return RedirectToAction("Planning");
        }
    }
}
// Render Force Build 3 Mei 2026