using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;
using Newtonsoft.Json;
namespace WebSale.Controllers
{
    public class ProductsController : Controller
    {
        // GET: ProductDetails
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();

        [Route("Products/{slug}")]
        public ActionResult Index(string slug)
        {
            var productsWithAllImages = db.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Type)
                .Where(p => p.slug == slug && p.status == 1)
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
                        MauSac = i.MauSac,
                        IsDefault = i.isDefault ?? false,
                    }).ToList()
                }).ToList();

            var productList = productsWithAllImages.ToList();
            var currentProductId = productList.FirstOrDefault()?.productId;
            var brandId = productList.FirstOrDefault()?.idBrand;

            var listSpCungLoai = db.Products
                .Where(item => item.brandId == brandId && item.productId != currentProductId && item.status == 1)
                .ToList();

            var product = db.Products
                .Include(p => p.ThongSoKyThuat)
                .FirstOrDefault(p => p.productId == currentProductId);

            if (productsWithAllImages.Any())
            {
                ViewBag.title = productsWithAllImages.First().nameProduct;
                ViewBag.listspcungloai = listSpCungLoai;
                ViewBag.listCauHinh = product;
            }

            var currentProduct = productsWithAllImages.First();

            HttpCookie listViewed = Request.Cookies["viewdProduct"];
            List<int> viewedProductIds = new List<int>();

            if (listViewed != null && !string.IsNullOrEmpty(listViewed.Value))
            {
                try
                {
                    // Chuyển chuỗi JSON thành danh sách
                    viewedProductIds = JsonConvert.DeserializeObject<List<int>>(listViewed.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing viewed product IDs: {ex.Message}");
                }
            }

            // Kiểm tra nếu sản phẩm hiện tại chưa có trong danh sách đã xem thì thêm vào
            if (currentProductId.HasValue && !viewedProductIds.Contains(currentProductId.Value))
            {
                viewedProductIds.Add(currentProductId.Value);
                // Lưu lại vào cookie
                var cookieValue = JsonConvert.SerializeObject(viewedProductIds);
                HttpCookie listViewedCookie = new HttpCookie("viewdProduct", cookieValue)
                {
                    Expires = DateTime.Now.AddDays(30) // Cookie có thời gian hết hạn là 30 ngày
                };
                Response.Cookies.Add(listViewedCookie); // Thêm cookie vào phản hồi
            }

            // Truy vấn các sản phẩm đã xem từ danh sách ID trong cookie
            var recentlyViewedProducts = db.Products
                .Where(p => viewedProductIds.Contains(p.productId) && p.status == 1)
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
                        MauSac = i.MauSac,
                        IsDefault = i.isDefault ?? false,
                    }).ToList()
                }).ToList();

            ViewBag.recentlyViewedProducts = recentlyViewedProducts.Take(5);

            var reviews = db.Reviews.Where(r => r.productid == currentProductId).Include("User").ToList();
            int totalReviews = reviews.Count;


            double averageRating = reviews.Any() ? reviews.Average(r => r.star) : 0;

            int[] ratingCounts = new int[5];

            // 0 0 0 1 3
            foreach (var review in reviews)
            {
                if (review.star >= 1 && review.star <= 5)
                {
                    ratingCounts[review.star - 1]++;
                }
            }

            ViewBag.AverageRating = Math.Round(averageRating, 1);
            ViewBag.TotalReviews = totalReviews;
            ViewBag.RatingCounts = ratingCounts;
            ViewBag.listReview = reviews;

            return View(productList);
        }
    }
}