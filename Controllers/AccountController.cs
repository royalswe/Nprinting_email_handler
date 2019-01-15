using AnalyticConfig.DAL;
using AnalyticConfig.Models;
using AnalyticConfig.ViewModels;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AnalyticConfig.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public ActionResult Routing()
        {
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // GET: Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Username, Password")] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Inloggningen misslyckades";
                return View(model);
            }

            SystemAdmin user = AuthRepository.Authenticate(model.Username, model.Password);

            if (user != null)
            {
                FormsAuthentication.SetAuthCookie(user.Username, false);

                var authTicket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddMinutes(60), false, user.Role);
                string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                HttpContext.Response.Cookies.Add(authCookie);
                return RedirectToAction("Routing");
            }
            else
            {
                TempData["error"] = "Inloggningen misslyckades";
                return View(model);
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            TempData["success"] = "Du är nu utloggad";
            FormsAuthentication.SignOut();

            if (Request.Cookies["adminCoockie"] != null)
            {
                var c = new HttpCookie("adminCoockie");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }

            return RedirectToAction("Login", "Account");
        }

    }
}
