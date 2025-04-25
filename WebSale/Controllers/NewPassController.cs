using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Controllers
{
    public class NewPassController : Controller
    {
        // GET: NewPass
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index(string email , string token)
        {
            var checkUser = db.Users.Where(item => item.email == email && item.token == token).FirstOrDefault();
            if (checkUser != null)
            {
                ViewBag.Email = email;
                ViewBag.Token = token;
            }
            else
            {
                return Redirect("/khongtimthaytrang");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(string email, string password, string repassword, string token)
        {
            var checkUser = db.Users.FirstOrDefault(item => item.email == email && item.token == token);
            var resetLink = $"{Request.Url.Scheme}://{Request.Url.Authority}/NewPass?email={email}&token={token}";
            if (checkUser != null)
            {
                if (password == repassword)
                {
                    try
                    {
                        var passwordHasher = new PasswordHasher<User>();
                        checkUser.password = passwordHasher.HashPassword(checkUser, password);
                        checkUser.token = Guid.NewGuid().ToString(); 
                        db.SaveChanges();

                        TempData["success"] = "Cập nhật mật khẩu thành công";
                        return View();
                    }
                    catch (Exception ex)
                    {
                        TempData["error"] = "Đã xảy ra lỗi khi cập nhật mật khẩu: " + ex.Message;
                        return Redirect("/khongtimthaytrang");
                    }
                }
                else
                {
                    TempData["error"] = "Mật khẩu và xác nhận mật khẩu không khớp.";
                    return Redirect(resetLink);
                }
            }
            else
            {
                return Redirect("/khongtimthaytrang");
            }
        }
    }
}