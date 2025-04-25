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
    public class CategoryController : Controller
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        // GET: Category
        [Route("Category/{slug}")]
        public ActionResult Index(string slug)
        {
            var cate = db.Categories.FirstOrDefault(item => item.slug == slug);
            if (cate != null)
            {
                ViewBag.title = cate.nameCategory;
                ViewBag.slug = slug;
            }
            else
            {
                return Redirect("/khongtimthaytrang");
            }
            var productsWithAllImages = db.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Type)
            .Where(p => p.Category.slug == slug && p.status == 1)
            .Select(p => new ProductViewModel
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
                brandName = p.Brand.nameBrand,
                idBrand = p.Brand.brandId,
                typeName = p.Type.typeName,
                idType = p.Type.typeId,
                Image = p.ProductImages.Select(i => new ProductImageViewModel
                {
                    ImageId = i.imageId,
                    image = i.imageUrl,
                    Price = (decimal?)i.price ?? 0,
                    Sale = (decimal?)i.sale_price ?? 0,
                    IsDefault = i.isDefault ?? false,
                }).ToList()
            }).ToList();

            var productList = productsWithAllImages.ToList();

            var brandList = productList
                 .GroupBy(p => new { p.brandName, p.idBrand })
                 .Select(g => g.First())
                 .ToList();

            var typeList = productList
                 .GroupBy(p => new { p.typeName, p.idType })
                 .Select(g => g.First())
                 .ToList();

            ViewBag.Brands = brandList;
            ViewBag.Types = typeList;

            return View(productList);
        }
        [HttpPost]
        [Route("Category/{slug}")]
        public ActionResult Index(string type, string slug, decimal? minPrice, decimal? maxPrice, int? brand, int? typeId)
        {
            var products = from product in db.Products
                           join category in db.Categories on product.categoryId equals category.categoryId
                           join brandEntity in db.Brands on product.brandId equals brandEntity.brandId
                           join typeEntity in db.Types on product.typeId equals typeEntity.typeId
                           where category.slug == slug
                           select new ProductViewModel
                           {
                               productId = product.productId,
                               nameProduct = product.nameProduct,
                               slug = slug,
                               price = product.price,
                               sale_price = (decimal)product.sale_price,
                               percen_sale = (int)product.percen_sale,
                               image = product.image,
                               categoryName = category.nameCategory,
                               brandName = brandEntity.nameBrand,
                               typeName = typeEntity.typeName,
                               slugBrand = brandEntity.slug,
                               idBrand = brandEntity.brandId,
                               idType = typeEntity.typeId,
                           };

            // Apply filtering
            if (brand.HasValue)
            {
                products = products.Where(p => p.idBrand == brand.Value);
            }

            if (typeId.HasValue)
            {
                products = products.Where(p => p.idType == typeId.Value);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.price <= maxPrice.Value);
            }

            // Sorting logic
            if (type == "asc")
            {
                products = products.OrderBy(p => p.price);
            }
            else if (type == "desc")
            {
                products = products.OrderByDescending(p => p.price);
            }
            else if (type == "news")
            {
                //products = products.OrderByDescending(p => p.created_at);
            }

            // Retrieve final product list
            var productList = products.ToList();

            var cate = db.Categories.FirstOrDefault(item => item.slug == slug);
            ViewBag.title = cate?.nameCategory;

            // Group brands and types for filters
            var brandList = productList
                .GroupBy(p => new { p.brandName, p.idBrand })
                .Select(g => g.First())
                .ToList();

            var typeList = productList
                .GroupBy(p => new { p.typeName, p.idType })
                .Select(g => g.First())
                .ToList();

            // Set ViewBag properties
            ViewBag.Brands = brandList;
            ViewBag.Types = typeList;
            ViewBag.type = type;
            ViewBag.slug = slug;

            return View(productList);
        }
    }
}