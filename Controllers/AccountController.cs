using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
namespace VIP_Planning.Controllers {
    public class AccountController : Controller {
        public IActionResult Login() => View();

        public IActionResult AdminBypass(string pin) {
            if (pin == "3991") {
                // Hier vullen we nu jouw juiste MSN adres in
                HttpContext.Session.SetString("IsLoggedIn", "true");
                HttpContext.Session.SetString("UserEmail", "richard1990_6@msn.com");
                HttpContext.Session.SetString("UserName", "Richard");
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login");
        }
    }
}