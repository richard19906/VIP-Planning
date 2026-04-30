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
        public HomeController(Supabase.Client supabase) { _supabase = supabase; }

        public IActionResult Index() => View();
        public IActionResult Instellingen() => View();

        public async Task<IActionResult> Planning()
        {
            var profielen = await _supabase.From<ProfielModel>().Get();
            ViewBag.Werknemers = profielen.Models ?? new List<ProfielModel>();

            var conceptJson = HttpContext.Session.GetString("ConceptRooster");
            var conceptLijst = string.IsNullOrEmpty(conceptJson)
                ? new List<UrenModel>()
                : JsonConvert.DeserializeObject<List<UrenModel>>(conceptJson);

            return View(conceptLijst);
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

            // AANPASSING: OrderByDescending zorgt dat de nieuwste datum bovenaan staat
            var gefilterd = (response.Models ?? new List<UrenModel>())
                .Where(x => x.PeriodeNaam.Equals(maand, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => DateTime.ParseExact(x.DatumString, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .ToList();

            return View(gefilterd);
        }

        [HttpPost]
        public IActionResult OpslaanLokaal(UrenModel nieuw)
        {
            var conceptJson = HttpContext.Session.GetString("ConceptRooster");
            var lijst = string.IsNullOrEmpty(conceptJson)
                ? new List<UrenModel>()
                : JsonConvert.DeserializeObject<List<UrenModel>>(conceptJson);

            if (DateTime.TryParse(nieuw.DatumString, out DateTime d))
            {
                var culture = new CultureInfo("nl-NL");
                nieuw.DatumString = d.ToString("dd-MM-yyyy");

                DateTime periodeDatum = d.Day >= 21 ? d.AddMonths(1) : d;
                string m = periodeDatum.ToString("MMM", culture).Replace(".", "");
                nieuw.PeriodeNaam = culture.TextInfo.ToTitleCase(m);
            }

            lijst.Add(nieuw);
            HttpContext.Session.SetString("ConceptRooster", JsonConvert.SerializeObject(lijst));
            return RedirectToAction("Planning");
        }

        [HttpPost]
        public async Task<IActionResult> PushNaarDatabase()
        {
            var conceptJson = HttpContext.Session.GetString("ConceptRooster");
            if (!string.IsNullOrEmpty(conceptJson))
            {
                var lijst = JsonConvert.DeserializeObject<List<UrenModel>>(conceptJson);
                foreach (var item in lijst)
                {
                    await _supabase.From<UrenModel>().Insert(item);
                }
                HttpContext.Session.Remove("ConceptRooster");
            }
            return RedirectToAction("Planning");
        }
    }
}