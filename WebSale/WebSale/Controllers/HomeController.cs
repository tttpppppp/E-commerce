using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;
namespace WebSale.Controllers
{
    public class HomeController : Controller
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        private IDatabase redisDatabase;
        public ActionResult Index()
        {
            mapBrand mapBrand = new mapBrand();
            mapCategories mapCategories = new mapCategories();
            try
            {
              var productsWithAllImages =
              from p in db.Products
              join img in db.ProductImages on p.productId equals img.productId into imgGroup
              where p.status == 1
              select new ProductViewModel
              {
                  productId = p.productId,
                  nameProduct = p.nameProduct,
                  price = (decimal?)p.price ?? 0,
                  sale_price = (decimal?)p.sale_price ?? 0,
                  percen_sale = (decimal?)p.percen_sale ?? 0,
                  des = p.description,
                  slug = p.slug,
                  quantity = (int)p.quantity,
                  sold_quantity = (int?)p.sold_quantity ?? 0,
                  image = p.image,
                  Image = imgGroup.Select(i => new ProductImageViewModel
                  {
                      ImageId = i.imageId,
                      image = i.imageUrl,
                      Price = (decimal?)i.price ?? 0,
                      Sale = (decimal?)i.sale_price ?? 0,
                      MauSac = (string)i.MauSac,
                      IsDefault = i.isDefault ?? false, 
                  }).ToList()
              };

                ViewBag.listProduct = productsWithAllImages;
            }
            catch
            {

            }
            ViewBag.categories = db.Categories.Where(item => item.status == 1).ToList();
            return View();
        }
    }
}