using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Controllers
{
    public class DangKiController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Index(User user)
        {
            using (WebBanHangASPEntities3 db = new WebBanHangASPEntities3())
            {

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                         .Where(m => m.Value.Errors.Any())
                         .ToDictionary(
                             kvp => kvp.Key,
                             kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
                         );

                    return Json(new { isSuccess = false, errors });
                }

                var findEmail = db.Users.FirstOrDefault(item => item.email == user.email);
                if (findEmail != null) // Changed to null check for better clarity
                {
                    return Json(new { isSuccess = false, error = "Email đã tồn tại!" });
                }

                var passwordHasher = new PasswordHasher<User>();
                user.password = passwordHasher.HashPassword(user, user.password);

                db.Users.Add(user);
                db.SaveChanges();
                return Json(new { isSuccess = true });
            }
        }

    }
}