using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Admin/Categories
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            mapCategories mapCategories = new mapCategories();
            return View(mapCategories.getCategories());
        }
        public ActionResult addCategories()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult addCategories(Category category, HttpPostedFileBase file)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            if (string.IsNullOrEmpty(category.nameCategory))
            {
                ModelState.AddModelError("nameCategory", "Trường tên không được để trống!");
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

            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            category.created_at = DateTime.Now;
            category.updated_at = DateTime.Now;
            category.imageCategory = path + "/" + filename2;
            db.Categories.Add(category);
            db.SaveChanges();
            TempData["success"] = "Thêm danh mục thành công!";
            return RedirectToAction("Index");
        }
        public ActionResult deleteCategory(int? id)
        {
            if (id == null)
            {
                return Redirect("/Admin/PageNotFound/Index");
            }
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var category = db.Categories.Find(id);
            var product = db.Products.Any(p => p.brandId == id);
            if (product)
            {
                category.status = 0;
            }
            else
            {
                db.Categories.Remove(category);
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult updateCategory(int? id)
        {
            if (id == null)
            {
                return Redirect("/Admin/PageNotFound/Index");
            }
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var category = db.Categories.Find(id);
            return View(category);
        }
        [HttpPost]
        public ActionResult updatecategory(Category category, HttpPostedFileBase file)
        {
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            var cate = db.Categories.Find(category.categoryId);
            var image = cate.imageCategory;
            if (file != null)
            {
                string path = "/uploads";
                string filename = file.FileName;
                string ext = System.IO.Path.GetExtension(filename);
                string filename2 = new Random().Next(1000, 10000).ToString() + ext;
                string rootPath = Server.MapPath(path);
                string savePath = rootPath + "/" + filename2;
                file.SaveAs(savePath);
                image = path + "/" + filename2;
            }
            cate.nameCategory = category.nameCategory;
            cate.status = category.status;
            cate.created_at = DateTime.Now;
            cate.slug = category.slug;
            cate.imageCategory = image;
            db.SaveChanges();
            TempData["success"] = "Cập nhật danh mục thành công!";
            return RedirectToAction("Index");
        }

    }
}