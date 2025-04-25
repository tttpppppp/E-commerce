using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class MucLucController : Controller
    {
        // GET: Admin/MucLuc
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        // GET: Admin/Blog
        public ActionResult Index()
        {
            var listmucluc = db.MucLucs.ToList();
            return View(listmucluc);
        }
        public ActionResult addMucLuc(DanhMucBlog danhMucBlog)
        {
            ViewBag.blog = db.Blogs.ToList();

            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult addMucLuc(MucLuc mucLuc)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            if (string.IsNullOrEmpty(mucLuc.tieu_de))
            {
                ModelState.AddModelError("Tên tiêu đề", "Trường tên không được để trống!");
                return View();
            }
            db.MucLucs.Add(mucLuc);
            db.SaveChanges();

            ViewBag.blog = db.Blogs.ToList();

            TempData["success"] = "Thêm nhãn hàng thành công!";
            return RedirectToAction("Index");
        }
    }
}