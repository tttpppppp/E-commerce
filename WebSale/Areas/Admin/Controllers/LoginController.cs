using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(string email , string password)
        {
            bool isError = false;
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var user = db.Admins.FirstOrDefault(u => u.email == email);
            if(string.IsNullOrWhiteSpace(email))
            {
                ViewBag.erroremail = "Tài khoản không được để trống";
               isError = true;
            }
            if(string.IsNullOrWhiteSpace(password))
            {
                ViewBag.errorpassword = "Mật khẩu không được để trống";
                isError =true;
            }
            if (isError) { 
                return View();
            }
            if (user == null)
            {
                ViewBag.error = "Email của bạn không đúng!";
                return View();
            }
            if (user.password != password) {
                ViewBag.error = "Mật khẩu của bạn không đúng!";
                return View();
            }
            var userAdmin = new AdminModel
            {
                Email = email,
                Image = user.image,
                FirstName = user.firstname
            };
            Session["admin"] = userAdmin;
            return Redirect("/Admin/Dashboard/");
        }
        public ActionResult logout() {
            Session["admin"] = null;

            if (Request.Cookies["AuthToken"] != null)
            {
                var authCookie = new HttpCookie("Email")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(authCookie);
            }
            return RedirectToAction("Index");
        }
    }
}