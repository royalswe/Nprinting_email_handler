using AnalyticConfig.DAL;
using AnalyticConfig.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AnalyticConfig.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        UserDAL userDAL = new UserDAL();
        private SystemAdmin authUser = AdminDal.GetAuthUser();
        List<User> users = new List<User>();

        // Get Help
        public ActionResult Help()
        {
            return View();
        }
        // Get Index
        public ActionResult Index(int? pagesize, string sort_roles)
        {          
            ViewBag.PageSize = pagesize ?? 20; // Default pagesize

            // To see modelstate errors on redirect
            if (TempData.ContainsKey("ModelState"))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

            users = userDAL.GetUsers();

            if (users != null)
            {
                ViewBag.schoolUnitList = users.GroupBy(s => s.SchoolUnit, i => i, (k, item) => new SelectListItem
                {
                    Text = item.First().SchoolUnit,
                    Value = item.First().SchoolUnit
                }).ToList();

                if (string.IsNullOrEmpty(sort_roles) == false)
                {
                    // Sort users by role
                    users = users
                        .Where(x => x.Role == sort_roles)
                        .Select(g => g)
                        .ToList();
                }
                else
                {
                    // Remove users with no name
                    users = users
                        .Where(x => !string.IsNullOrEmpty(x.Name))
                        .Select(g => g)
                        .ToList();
                }
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_WebGrid", users);
            }

            return View(users);
        }

        // POST: Home/Edit/id
        [HttpPost]
        public JsonResult Edit([Bind(Include = "Id, SchoolUnit, Name, Role, Email, Password")]User user)
        {
            string errorMessages = string.Join(" och ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
            int index = 0;
            string result = string.Empty;
            if (ModelState.IsValid)
            {
                try
                {
                    user = userDAL.UpdateUser(user);
                    users = userDAL.GetUsers();
                }
                catch
                {
                    result = "0";
                }

                index = users.FindIndex(u => u.Id == user.Id);
                if (index >= 0)
                {
                    users[index] = user;
                    result = "1";
                }
                else
                {
                    result = "0";
                }
            }
            else
            {
                result = errorMessages;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // POST: Home/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SchoolUnit, Name, Role, Email, Password")] User user)
        {
            if (ModelState.IsValid)
            {
                users = userDAL.GetUsers();
                // Check for duplicate users
                User duplicate = users.Find(u => (u.Email == user.Email) && (u.Role == user.Role) && (u.SchoolUnit == user.SchoolUnit));
                if (duplicate == null)
                {
                    user = userDAL.CreateUser(user);
                    TempData["success"] = string.Format("Mottagaren {0} har skapats", user.Name);
                }
                else
                {
                    TempData["error"] = "Mottagare med samma uppgifter finns redan";
                }
            }
            else
            {
                TempData["error"] = "Kunde inte skapa mottagaren";
                TempData["ModelState"] = ModelState;
            }
            return RedirectToAction("Index");

        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            string result = string.Empty;
            try
            {
                User user = userDAL.GetUser(id); // Check if user exist
                
                if (user != null)
                {
                    userDAL.DeleteUser(user.Id);
                    result = "1";
                }
                else
                {
                    result = "0";
                }
                    
            }
            catch(Exception ex)
            {
                result = ex.Message;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}