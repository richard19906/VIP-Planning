using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() => View();
        public IActionResult Planning() => View();
        public IActionResult Werknemers() => View(); // Werknemers terug!
        public IActionResult Instellingen() => View();

        [HttpPost]
        public IActionResult SavePincode(string newPin) {
            // Logica voor pincode opslaan
            return RedirectToAction("Instellingen");
        }

        [HttpPost]
        public IActionResult UpdatePassword(string oldPw, string newPw) {
            // Logica voor wachtwoord wijzigen
            return RedirectToAction("Instellingen");
        }
    }
}