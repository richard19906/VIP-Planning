using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Supabase;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using VIP_Planning.Models;

namespace VIP_Planning.Controllers
{
    public class HomeController : Controller
    {
        private readonly Supabase.Client _supabase;

        public HomeController(Supabase.Client supabase) { _supabase = supabase; }

        // --- DASHBOARD ---
        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                return RedirectToAction("Login", "Account");
            return View();
        }

        // --- INSTELLINGEN (Deze lost de 404 op) ---
        public IActionResult Instellingen()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                return RedirectToAction("Login", "Account");

            return View(); // Zorg dat Views/Home/Instellingen.cshtml bestaat!
        }

        // --- WERKNEMERS ---
        public async Task<IActionResult> Werknemers()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                return RedirectToAction("Login", "Account");

            var response = await _supabase.From<ProfielModel>().Get();
            return View(response.Models ?? new List<ProfielModel>());
        }

        // --- UREN OVERZICHT ---
        [HttpGet]
        public async Task<IActionResult> UrenOverzicht(string email, string naam, string maand)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                return RedirectToAction("Login", "Account");

            var culture = new CultureInfo("nl-NL");
            ViewBag.Maanden = new List<string> { "Jan", "Feb", "Mrt", "Apr", "Mei", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dec" };

            if (string.IsNullOrEmpty(maand))
                maand = culture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMM", culture).Replace(".", ""));

            ViewBag.GeselecteerdeMaand = maand;
            ViewBag.Email = email;
            ViewBag.Naam = naam;

            var response = await _supabase.From<UrenModel>().Where(x => x.UserEmail == email).Get();
            var alleUren = response.Models ?? new List<UrenModel>();

            var gefilterdeUren = alleUren
                .Where(u => u.PeriodeNaam != null && u.PeriodeNaam.Equals(maand, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(u => u.DatumString)
                .ToList();

            return View(gefilterdeUren);
        }

        // --- PLANNING BEHEER ---
        public async Task<IActionResult> Planning(string maand)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
                return RedirectToAction("Login", "Account");

            var culture = new CultureInfo("nl-NL");
            var profielen = await _supabase.From<ProfielModel>().Get();
            ViewBag.Werknemers = profielen.Models ?? new List<ProfielModel>();

            if (string.IsNullOrEmpty(maand))
                maand = culture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMM", culture).Replace(".", ""));

            ViewBag.GeselecteerdeMaand = maand;

            var response = await _supabase.From<PlanningModel>().Get();
            var lijst = response.Models ?? new List<PlanningModel>();

            var gefilterd = lijst.Where(x => DateTime.TryParse(x.Datum, out DateTime d) &&
                d.ToString("MMM", culture).Replace(".", "").Equals(maand, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.Datum).ToList();

            return View(gefilterd);
        }

        [HttpPost]
        public async Task<IActionResult> OpslaanLokaal(PlanningModel nieuw)
        {
            string datumInvoer = Request.Form["DatumString"];
            var profielen = await _supabase.From<ProfielModel>().Get();
            var profiel = profielen.Models?.FirstOrDefault(p => p.Email == nieuw.UserEmail);

            string dbDatum = DateTime.TryParse(datumInvoer, out DateTime d) ? d.ToString("yyyy-MM-dd") : nieuw.Datum;

            if (nieuw.Id == 0)
            {
                var item = new PlanningModel
                {
                    UserEmail = nieuw.UserEmail,
                    UserNaam = profiel?.Naam ?? "Onbekend",
                    Locatie = nieuw.Locatie,
                    Uren = nieuw.Uren,
                    Datum = dbDatum,
                    StartTijd = nieuw.StartTijd ?? "00:00",
                    EindTijd = nieuw.EindTijd ?? "00:00",
                    Status = "Gepland"
                };
                await _supabase.From<PlanningModel>().Insert(item);
            }
            else
            {
                nieuw.UserNaam = profiel?.Naam ?? "Onbekend";
                nieuw.Datum = dbDatum;
                await _supabase.From<PlanningModel>().Upsert(nieuw);
            }
            return RedirectToAction("Planning");
        }

        public async Task<IActionResult> Verwijderen(int id)
        {
            await _supabase.From<PlanningModel>().Where(x => x.Id == id).Delete();
            return RedirectToAction("Planning");
        }
    }
}