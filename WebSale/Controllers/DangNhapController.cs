using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Services.Description;
using WebSale.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace WebSale.Controllers
{
    public class DangNhapController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult Index(User u)
        {
            bool isSuccess = false;
            var errors = new Dictionary<string, List<string>>();

            if (string.IsNullOrEmpty(u.email))
            {
                errors["email"] = new List<string> { "Vui lòng nhập email" };
            }
            else if (!new EmailAddressAttribute().IsValid(u.email))
            {
                errors["email"] = new List<string> { "Email không hợp lệ" };
            }

            // Validate password
            if (string.IsNullOrEmpty(u.password))
            {
                errors["password"] = new List<string> { "Vui lòng nhập mật khẩu" };
            }

            if (errors.Any())
            {
                return Json(new { isSuccess = false, errors });
            }

            using (var db = new WebBanHangASPEntities3())
            {
                var user = db.Users.FirstOrDefault(item => item.email == u.email);
                if (user == null)
                {
                    return Json(new { isSuccess = false, error = "Email của bạn không đúng" });
                }
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.password, u.password);
                if (result == PasswordVerificationResult.Failed)
                {
                    return Json(new { isSuccess = false, error = "Password của bạn không đúng" });
                }
                var userSession = new UserModel
                {
                    FirstName = user.firstName,
                    Email = user.email,
                    Id = user.userId,
                };
                Session["user"] = userSession;
                isSuccess = true;
            }

            return Json(new { isSuccess });
        }
        [HttpPost]
        public JsonResult logout()
        {
            Session["user"] = null;
            return Json(new { isSuccess = true });
        }
    }
}