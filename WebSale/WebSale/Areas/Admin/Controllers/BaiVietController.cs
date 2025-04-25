using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class BaiVietController : Controller
    {
        // GET: Admin/BaiViet
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            var listBaiViet = db.Blogs.ToList();
            return View(listBaiViet);
        }
        public ActionResult addBaiViet(Blog blog)
        {
            ViewBag.listcategory = db.DanhMucBlogs.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult addBaiVietPost(Blog blog , HttpPostedFileBase file)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            if (string.IsNullOrEmpty(blog.tieu_de))
            {
                ModelState.AddModelError("Tên tiêu đề", "Trường tên không được để trống!");
                return View("addBaiViet");
            }
            if (file == null)
            {
                return View();
            }
            if (file.ContentLength < 0)
            {
                return View();
            }
            string path = "/uploads";
            string filename = file.FileName;
            string ext = System.IO.Path.GetExtension(filename);
            string filename2 = new Random().Next(1000, 10000).ToString() + ext;
            string rootPath = Server.MapPath(path);
            string savePath = rootPath + "/" + filename2;
            file.SaveAs(savePath);
            blog.image_thumb = path + "/" + filename2;
            blog.ngay_dang = DateTime.Now;
            db.Blogs.Add(blog);
            db.SaveChanges();

            ViewBag.listcategory = db.DanhMucBlogs.ToList();

            TempData["success"] = "Thêm bài viết thành công!";
            return RedirectToAction("Index");
        }
    }
}