using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VIP_Planning.Controllers {
    public class HomeController : Controller {
        public IActionResult Index() => View();
        public IActionResult Planning() => View();
        public IActionResult Instellingen() => View();

        [HttpPost]
        public IActionResult SavePincode(string newPin) {
            return RedirectToAction("Instellingen");
        }

        [HttpPost]
        public IActionResult UpdatePassword(string oldPw, string newPw) {
            return RedirectToAction("Instellingen");
        }
    }
}