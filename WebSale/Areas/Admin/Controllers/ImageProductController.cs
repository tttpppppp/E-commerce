using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class ImageProductController : Controller
    {
        // GET: Admin/ImageProduct
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            var listProduct = db.Products.ToList();
            ViewBag.Products = listProduct;
            return View();
        }
        [HttpPost]
        public ActionResult Index(ProductImage productImage , HttpPostedFileBase file)
        {
            var listProduct = db.Products.ToList();
            ViewBag.Products = listProduct;

            string path = "/uploads";
            string filename = file.FileName;
            string ext = System.IO.Path.GetExtension(filename);
            string filename2 = new Random().Next(1000, 10000).ToString() + ext;
            string rootPath = Server.MapPath(path);
            string savePath = rootPath + "/" + filename2;
            file.SaveAs(savePath);
            productImage.imageUrl = path + "/" + filename2;
            productImage.productId = productImage.productId;
            db.ProductImages.Add(productImage);
            db.SaveChanges();
            return View();
        }
    }
}