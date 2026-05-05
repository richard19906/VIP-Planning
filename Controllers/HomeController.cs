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

        public HomeController(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        public IActionResult Index()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        public async Task<IActionResult> Planning(string maand)
        {
            var culture = new CultureInfo("nl-NL");
            var profielen = await _supabase.From<ProfielModel>().Get();
            ViewBag.Werknemers = profielen.Models ?? new List<ProfielModel>();

            if (string.IsNullOrEmpty(maand))
            {
                maand = culture.TextInfo.ToTitleCase(DateTime.Now.ToString("MMM", culture).Replace(".", ""));
            }
            ViewBag.GeselecteerdeMaand = maand;

            var response = await _supabase.From<PlanningModel>().Get();
            var planningLijst = response.Models ?? new List<PlanningModel>();

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

        [HttpPost]
        public async Task<IActionResult> OpslaanLokaal(PlanningModel nieuw)
        {
            string datumInvoer = Request.Form["DatumString"];
            var profielenResponse = await _supabase.From<ProfielModel>().Get();
            var werknemerProfiel = profielenResponse.Models?.FirstOrDefault(p => p.Email == nieuw.UserEmail);

            string dbDatum = "";
            if (DateTime.TryParse(datumInvoer, out DateTime d))
            {
                dbDatum = d.ToString("yyyy-MM-dd");
            }
            else
            {
                dbDatum = nieuw.Datum;
            }

            var roosterItem = new PlanningModel
            {
                Id = nieuw.Id > 0 ? nieuw.Id : 0, // Bij 0 maakt Supabase een nieuw ID aan
                UserEmail = nieuw.UserEmail,
                UserNaam = werknemerProfiel?.Naam ?? "Onbekend",
                Locatie = nieuw.Locatie,
                Uren = nieuw.Uren,
                Datum = dbDatum,
                StartTijd = nieuw.StartTijd ?? "08:00",
                EindTijd = nieuw.EindTijd ?? "17:00",
                IsGepusht = false,
                Status = "Gepland"
            };

            await _supabase.From<PlanningModel>().Upsert(roosterItem);

            var culture = new CultureInfo("nl-NL");
            return RedirectToAction("Planning", new { maand = ViewBag.GeselecteerdeMaand });
        }

        // GEFIXT: Naam is nu 'Verwijderen' en accepteert GET-verzoeken (geen [HttpPost])
        public async Task<IActionResult> Verwijderen(int id)
        {
            await _supabase.From<PlanningModel>().Where(x => x.Id == id).Delete();
            return RedirectToAction("Planning");
        }
    }
}