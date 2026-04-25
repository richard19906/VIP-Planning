using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() => View();
        
        public IActionResult Planning() => View();
        
        public IActionResult Werknemers() => View();

        public IActionResult UrenOverzicht(string naam) {
            ViewBag.Naam = naam;
            return View();
        }

        public IActionResult Instellingen() => View();

        [HttpPost]
        public IActionResult SavePlanning(string medewerker, string locatie, string datum, string uren) {
            // Logica om naar Supabase 'planning' tabel te schrijven
            return RedirectToAction("Planning");
        }

        public IActionResult ExportUrenPdf(string naam) {
            // PDF Engine trigger specifiek voor uren van een werknemer
            return File(new byte[0], "application/pdf", $"Urenoverzicht-{naam}.pdf");
        }
    }
}