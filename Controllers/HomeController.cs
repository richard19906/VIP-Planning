using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
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
        public IActionResult SavePincode(string newPin) {
            // Hier komt de database push voor de pincode
            return RedirectToAction("Instellingen");
        }

        [HttpPost]
        public IActionResult UpdatePassword(string oldPw, string newPw) {
            // Hier komt de Supabase Auth update
            return RedirectToAction("Instellingen");
        }
        
        public IActionResult ExportPlanningPdf() {
            // PDF Engine trigger
            return File(new byte[0], "application/pdf", "VIP-Planning.pdf");
        }
    }
}