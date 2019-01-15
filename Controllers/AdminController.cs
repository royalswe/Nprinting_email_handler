using AnalyticConfig.DAL;
using AnalyticConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

/// <summary>
/// This Controller is specifik for handling the admins
/// </summary>

namespace AnalyticConfig.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        List<SystemAdmin> adminUsers = AdminDal.GetAdminUsers();
        // GET: Admin
        public ActionResult Index()
        {        
            return View(adminUsers);
        }
        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Creates new admins
        /// </summary>
        // POST: /Admin/Register
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Register(SystemAdmin user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AdminDal.CreateAdmin(user);
                    TempData["success"] = string.Format("Användaren {0} har skapats", user.Username);
                }
                catch (Exception ex)
                {
                    TempData["error"] = string.Format("Felmeddelande: {0}", ex.Message);
                }
            }

            return View(user);
        }

        /// <summary>
        /// Takes the username parameter and creates a session coockie. 
        /// This will make the admin signed in as the user and can see all of the users content. 
        /// </summary>
        /// GET: Admin/Details/id
        public ActionResult Details(string username)
        {
            try
            {
                int hours = 8;
                //create a cookie
                HttpCookie adminCoockie = new HttpCookie("adminCoockie");
                //Add key-values in the cookie
                adminCoockie.Values.Add("username", username);
                adminCoockie.Expires = DateTime.Now.AddHours(hours);
                //Most important, write the cookie to client.
                Response.Cookies.Add(adminCoockie);

                TempData["success"] = string.Format("Ändringar kommer identifieras som {0}", username);

                return RedirectToAction("Index", "Home");
            }
            catch
            {
                TempData["error"] = string.Format("kunde inte skapa kakor för {0}", username);
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }
        }

        // GET: Admin/Edit/id
        public ActionResult Edit(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            SystemAdmin user = adminUsers.FirstOrDefault(s => s.Id.Equals(id));

            if (user == null)
            {
                TempData["error"] = "Hittade inte vald administratör";
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }

            return View(user);
        }

        // POST: Admin/Edit/id
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            TempData["error"] = "fel";
            try
            {
                SystemAdmin user = adminUsers.FirstOrDefault(s => s.Id.Equals(id));
                UpdateModel(user);
                AdminDal.UpdateAdmin(id, user);

                TempData["success"] = string.Format("{0} Uppdaterades", user.Username);
                return RedirectToAction("Edit", new { id = user.Id });                             
            }
            catch (Exception ex)
            {
                TempData["error"] = string.Format("fel: {0}", ex.Message);
                return RedirectToAction("Edit", new { id });
            }
        }

        // POST: Admin/Delete/id
        [HttpPost]
        public ActionResult Delete(int id)
        {
            SystemAdmin user = adminUsers.First(s => s.Id.Equals(id));

            if (user != null)
            {
                AdminDal.DeleteAdmin(user.Id);
                TempData["success"] = string.Format("{0} raderades", user.Username);
            }
            else
            {
                TempData["error"] = "Kunde inte radera adminstratören";
            }
                
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Runs when Clicks the "Hämta data" button
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public ActionResult FetchFromFile()
        {
            try
            {
                new FetchFromFile();
                TempData["success"] = "Laddningen är färdig";
            }
            catch
            {
                TempData["error"] = "Laddningen misslyckades";
            }
            return Redirect("Index");
        }
    }
}
