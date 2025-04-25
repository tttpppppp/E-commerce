using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Controllers
{
    public class FilterCategoryController : Controller
    {
        // GET: FilterCategory
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        [Route("FilterCategory/Index")]
        public JsonResult Index(string type, string slug, decimal? minPrice, decimal? maxPrice , int? brand , int? typeId)
        {
            var products = db.Products.Where(p => p.status == 1).ToList();
            if (!string.IsNullOrEmpty(slug))
            {
                products = products.Where(p => p.Category.slug == slug).ToList();
            }
            if (brand.HasValue)
            {
                products = products.Where(p => p.Brand.brandId == brand).ToList();
            }if (typeId.HasValue)
            {
                products = products.Where(p => p.Type.typeId == typeId).ToList();
            }
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                products = products.Where(p => p.price >= minPrice && p.price <= maxPrice).ToList();
            }
            else if (minPrice.HasValue)
            {
                products = products.Where(p => p.price >= minPrice).ToList();
            }
            else if (maxPrice.HasValue)
            {
                products = products.Where(p => p.price <= maxPrice).ToList();
            }
            if (type == "asc")
            {
                products = products.OrderBy(p => p.price).ToList();
            }
            else if (type == "desc")
            {
                products = products.OrderByDescending(p => p.price).ToList();
            }
            else if (type == "news")
            {
                products = products.OrderByDescending(p => p.created_at).ToList();
            }

            var productList = products.Select(item => new
            {
                item.productId,
                item.slug,
                item.image,
                item.nameProduct,
                item.price,
                sale_price = item.sale_price,
                item.percen_sale
            }).ToList();
            var cate = db.Categories.FirstOrDefault(item => item.slug == slug);
            ViewBag.title = cate.nameCategory;
            return Json(productList, JsonRequestBehavior.AllowGet);
        }

    }
}