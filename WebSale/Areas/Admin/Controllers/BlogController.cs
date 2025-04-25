using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class BlogController : Controller
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        // GET: Admin/Blog
        public ActionResult Index()
        {
            var listdanhmuc = db.DanhMucBlogs.ToList();
            return View(listdanhmuc);
        }
        public ActionResult addDanhMuc(DanhMucBlog danhMucBlog) { 
            return View();
        }
        [HttpPost]
        public ActionResult addDanhMuc(DanhMucBlog danhMucBlog , HttpPostedFileBase file) {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            if (string.IsNullOrEmpty(danhMucBlog.ten))
            {
                ModelState.AddModelError("Tên danh mục", "Trường tên không được để trống!");
                return View();
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
            danhMucBlog.hinh_anh = path + "/" + filename2;
            db.DanhMucBlogs.Add(danhMucBlog);
            db.SaveChanges();

            TempData["success"] = "Thêm nhãn hàng thành công!";
            return RedirectToAction("Index");
        }
    }
}