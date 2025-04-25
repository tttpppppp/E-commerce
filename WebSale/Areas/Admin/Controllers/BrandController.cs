using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{  
    
    public class BrandController : Controller
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        // GET: Admin/Brand
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            mapBrand mapBrand = new mapBrand();
            var listBrand = mapBrand.getAllBrand();
            return View(listBrand);
        }
        public ActionResult addBrand()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult addBrand(Brand brand , HttpPostedFileBase file)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            if (string.IsNullOrEmpty(brand.nameBrand))
            {
                ModelState.AddModelError("nameBrand", "Trường tên không được để trống!");
                return View();
            }
            if (file == null)
            {
                return View();
            }
            if (file.ContentLength < 0) { 
                return View();
            }
            string path = "/uploads";
            string filename = file.FileName;
            string ext = System.IO.Path.GetExtension(filename);
            string filename2 = new Random().Next(1000, 10000).ToString() + ext;
            string rootPath = Server.MapPath(path);
            string savePath = rootPath + "/" + filename2;
            file.SaveAs(savePath);
            brand.created_at = DateTime.Now;
            brand.updated_at = DateTime.Now;
            brand.imageBrand = path + "/" + filename2;
            db.Brands.Add(brand);
            db.SaveChanges();

            TempData["success"] = "Thêm nhãn hàng thành công!";
            return RedirectToAction("Index");
        }
        public ActionResult updateBrand(int ?id)
        {

            if (id == null)
            {
                return Redirect("/Admin/PageNotFound/Index");
            }
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            var br = db.Brands.Find(id);
            return View(br);
        }
        [HttpPost]
        public ActionResult updateBrand(Brand brand , HttpPostedFileBase file)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            var br = db.Brands.Find(brand.brandId);
            br.nameBrand = brand.nameBrand;
            br.status = brand.status;
            br.updated_at = DateTime.Now;
            br.imageBrand = br.imageBrand;
            if (file != null)
            {
                string path = "/uploads";
                string filename = file.FileName;
                string ext = System.IO.Path.GetExtension(filename);
                string filename2 = new Random().Next(1000, 10000).ToString() + ext;
                string rootPath = Server.MapPath(path);
                string savePath = rootPath + "/" + filename2;
                file.SaveAs(savePath);
                br.imageBrand = path + "/" + filename2;
            }
            db.SaveChanges();
            TempData["success"] = "Cập nhật nhãn hàng thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult deleteBrand(int ?id) {
            if (id == null)
            {
                return Json(new { success = false });
            }
            if (Session["admin"] == null)
            {
                return Json(new { success = false });
            }
            var br = db.Brands.Find(id);
            var product = db.Products.Any(p => p.brandId == id);
            if (product) {
                br.status = 0;
            }
            else
            {
                db.Brands.Remove(br);
            }
            db.SaveChanges();
            return Json(new {success = true});
        }

    }
}