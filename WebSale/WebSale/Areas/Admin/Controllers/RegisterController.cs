using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Admin/Register
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Models.Admin admin, HttpPostedFileBase file)
        {
            // Kiểm tra nếu người dùng đã gửi email
            if (!string.IsNullOrEmpty(admin.email))
            {
                // Tìm kiếm admin theo email
                using (WebBanHangASPEntities3 db = new WebBanHangASPEntities3())
                {
                    var existingAdmin = db.Admins.FirstOrDefault(item => item.email == admin.email);
                    if (existingAdmin != null)
                    {
                        ViewBag.error = "Email đã tồn tại.";
                        return View(admin); 
                    }
                }
            }
            if (!ModelState.IsValid)
            {
                return View(admin); 
            }
            admin.image = null;
            admin.status = 1;
            admin.created_at = DateTime.Now;
            admin.updated_at = DateTime.Now;

            if (file != null)
            {
                string path = "/uploads";
                string filename = file.FileName;
                string ext = System.IO.Path.GetExtension(filename);
                string filename2 = new Random().Next(1000, 10000).ToString() + ext;
                string rootPath = Server.MapPath(path);
                string savePath = System.IO.Path.Combine(rootPath, filename2);
                file.SaveAs(savePath);
                admin.image = path + "/" + filename2;
            }

            // Thêm admin vào cơ sở dữ liệu
            using (WebBanHangASPEntities3 db = new WebBanHangASPEntities3())
            {
                db.Admins.Add(admin);
                db.SaveChanges();
            }

            ViewBag.success = "Đăng kí thành công!";
            return View();
        }
    }
}