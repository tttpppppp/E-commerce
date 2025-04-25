using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Admin/Profile
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var userAdmin = Session["admin"] as dynamic;
            var email = userAdmin.Email as string;
            var admin = db.Admins.FirstOrDefault(a => a.email == email);
            return View(admin);
        }
        [HttpPost]
        public ActionResult Index(string email , string firstname , string lastname , HttpPostedFileBase file)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var admin = db.Admins.SingleOrDefault(a => a.email == email);
            if (file == null)
            {
                admin.firstname = firstname;
                admin.lastname = lastname;
                admin.email = email;
                admin.updated_at = DateTime.Now;
                db.SaveChanges();
                return View(admin);
            }
            string path = "/uploads";
            string filename = file.FileName;
            string ext = System.IO.Path.GetExtension(filename);
            string filename2 = new Random().Next(1000, 10000).ToString() + ext;
            string rootPath = Server.MapPath(path);
            string savePath = rootPath + "/" + filename2;
            file.SaveAs(savePath);
            admin.firstname = firstname;
            admin.lastname = lastname;
            admin.email = email;
            admin.updated_at = DateTime.Now;
            admin.image = path + "/" + filename2;
            db.SaveChanges();
            var userAdmin = new AdminModel
            {
                Email = email,
                Image = admin.image,
                FirstName = admin.firstname
            };
            Session["admin"] = userAdmin;
            TempData["success"] = "Cập nhật thông tin thành công" ;
            return View(admin);
        }
    }
}