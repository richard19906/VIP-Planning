using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() => View();
        public IActionResult Planning() => View();
        public IActionResult Werknemers() => View();
        public IActionResult Instellingen() => View();

        [HttpPost]
        public IActionResult SavePincode(string newPin) {
            // Hier komt database save actie
            return RedirectToAction("Instellingen");
        }

        [HttpPost]
        public IActionResult UpdatePassword(string oldPw, string newPw) {
            // Hier komt password update actie via Supabase
            return RedirectToAction("Instellingen");
        }
    }
}