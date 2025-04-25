using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;
using Newtonsoft.Json;
using StackExchange.Redis;
namespace WebSale.Controllers
{
    public class ApiProductController : Controller
    {
        // GET: ApiProduct
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public JsonResult getAllProduct(string keyword)
        {
            try
            {
                var listpro = db.Products
                    .Select(p => new ProductViewModel
                    {
                        productId = p.productId,
                        nameProduct = p.nameProduct,
                        image = p.image,
                        price = p.price,
                        sale_price = p.sale_price ?? 0,
                        slug = p.slug,
                    }).ToList();

                if (!string.IsNullOrEmpty(keyword))
                {
                    keyword = keyword.ToLower();
                    listpro = listpro
                        .Where(item => item.nameProduct.ToLower().Contains(keyword))
                        .ToList(); 
                }

                return Json(new { success = true, products = listpro }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here for debugging
                return Json(new { success = false, message = "An error occurred while fetching products." }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getAllProductt4e()
        {
            try
            {
                //string cacheKey = "AllProducts";
                //IDatabase redisDb = MvcApplication.Redis.GetDatabase();

                //var cachedProducts = redisDb.StringGet(cacheKey);

                //if (cachedProducts.HasValue)
                //{
                //    var cachedData = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(cachedProducts);
                //    return Json(new { success = true, products = cachedData }, JsonRequestBehavior.AllowGet);
                //}

                // If cache is empty, fetch data from the database
                var productsWithAllImages = db.Products
             .Include(p => p.Category)
             .Include(p => p.Brand)
             .Include(p => p.Type)
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
                     MauSac = (string)i.MauSac,
                     IsDefault = i.isDefault ?? false,
                 }).ToList()
             }).ToList();

                var productList = productsWithAllImages.ToList();

                // Cache the data in Redis
                //var serializedProducts = JsonConvert.SerializeObject(productList);
                //redisDb.StringSet(cacheKey, serializedProducts, TimeSpan.FromMinutes(10)); // Set cache expiry as needed

                return Json(new { success = true, products = productList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception here for debugging if needed
                return Json(new { success = false, message = "An error occurred while fetching products." }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult getcart()
        {
            try
            {
                var listcart = Session["cart"] as List<CartItem> ?? new List<CartItem>();
                return Json(new { success = true, products = listcart }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Log the exception and return an error response
                System.Diagnostics.Debug.WriteLine("Error fetching cart: " + ex.Message);
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lấy giỏ hàng.", error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}