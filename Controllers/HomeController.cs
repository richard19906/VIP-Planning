using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() => View();
        
        // Planning met uren en medewerker info
        public IActionResult Planning() => View();
        
        // Werknemers met Voornaam, Achternaam en Email
        public IActionResult Werknemers() => View();

        // Specifiek urenoverzicht per persoon inclusief PDF trigger
        public IActionResult UrenOverzicht(string naam) {
            ViewBag.Naam = naam;
            return View();
        }

        public IActionResult Instellingen() => View();

        [HttpPost]
        public IActionResult SavePlanning(string medewerker, string locatie, string datum, string uren) {
            // Hier wordt de data naar de 'planning' tabel in Supabase gepusht
            return RedirectToAction("Planning");
        }

        public IActionResult ExportUrenPdf(string naam) {
            // PDF Engine voor het specifieke urenoverzicht
            return File(new byte[0], "application/pdf", $"Urenoverzicht-{naam}.pdf");
        }
    }
}