using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;
namespace WebSale.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        // GET: Admin/Product
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            mapProduct mapProduct = new mapProduct();
            return View(mapProduct.getAllProduct());
        }
        public ActionResult addProduct()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            ViewBag.listBrand = db.Brands.Where(item => item.status == 1).ToList();
            ViewBag.listCategory = db.Categories.Where(item => item.status == 1).ToList();
            ViewBag.listType = db.Types.Where(item => item.status == 1).ToList();
            ViewBag.listThongSo = db.ThongSoKyThuats.Where(item => item.status == 1).ToList();
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult addProduct(Product product , HttpPostedFileBase file)
        {
            bool isError = false;
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            if (string.IsNullOrEmpty(product.nameProduct))
            {
                ModelState.AddModelError("nameProduct", "Trường tên không được để trống!");
                isError = true;
            } 
            if (product?.price == null)
            {
                ModelState.AddModelError("price", "Trường tên không được để trống!");
                isError = true;
            }
            if (file == null)
            {
                isError = true;
            }
            ViewBag.listBrand = db.Brands.Where(item => item.status == 1).ToList();
            ViewBag.listCategory = db.Categories.Where(item => item.status == 1).ToList();
            ViewBag.listType = db.Types.Where(item => item.status == 1).ToList();
            ViewBag.listThongSo = db.ThongSoKyThuats.Where(item => item.status == 1).ToList();
            if (isError)
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
            product.created_at = DateTime.Now;
            product.updated_at = DateTime.Now;
            product.image = path + "/" + filename2;
            db.Products.Add(product);
            db.SaveChanges();
            TempData["success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Index");
        }
        public ActionResult deleteProduct(int ?id)
        {
            if (id == null)
            {
                return Redirect("/Admin/PageNotFound/Index");
            }
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }

            var sp = db.Products.Find(id);
            sp.status = 0;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult updateProduct(int? id)
        {
            if (id == null)
            {
                return Redirect("/Admin/PageNotFound/Index");
            }
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }

            ViewBag.listBrand = db.Brands.Where(item => item.status == 1).ToList();
            ViewBag.listCategory = db.Categories.Where(item => item.status == 1).ToList();
            ViewBag.listType = db.Types.Where(item => item.status == 1).ToList();
            ViewBag.listThongSo = db.ThongSoKyThuats.Where(item => item.status == 1).ToList();
            var sp = db.Products.Find(id);
            return View(sp);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult updateProduct(Product product , HttpPostedFileBase file)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }

            var sp = db.Products.Find(product.productId);
            var image = sp.image;
            if (file != null)
            {
                string path = "/uploads";
                string filename = file.FileName;
                string ext = System.IO.Path.GetExtension(filename);
                string filename2 = new Random().Next(1000, 10000).ToString() + ext;
                string rootPath = Server.MapPath(path);
                string savePath = rootPath + "/" + filename2;
                file.SaveAs(savePath);
                image =  path + "/" + filename2;
            }
            sp.nameProduct = product.nameProduct;
            sp.price = product.price;
            sp.quantity = product.quantity;
            sp.description = product.description;
            sp.status = product.status;
            sp.brandId = product.brandId;
            sp.typeId = product.typeId;
            sp.updated_at = DateTime.Now;
            sp.sku = product.sku;
            sp.percen_sale = product.percen_sale;
            sp.sale_price = product.sale_price;
            sp.specificationId = product.specificationId;
            sp.slug = product.slug;
            sp.categoryId = product.categoryId;
            sp.image = image;
            sp.isGift = product.isGift;
            db.SaveChanges();
            TempData["success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult addToCart(int id, int? quantity, int? imageId)
        {
            var success = false;
            var message = "Thêm thất bại";
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var product = db.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.productId == id &&
                                     (imageId == null || p.ProductImages.Any(pi => pi.imageId == imageId)));

            if (product == null || product.quantity <= 0)
            {
                return Json(new { success, message = "Sản phẩm đã hết hàng", count = cart.Count });
            }

            int itemQuantity = quantity.HasValue && quantity.Value > 0 ? quantity.Value : 1;

            var existingItem = cart.FirstOrDefault(p => p.Id == id &&
                                                        (imageId == null || p.mausac == product.ProductImages.FirstOrDefault(pi => pi.imageId == imageId)?.MauSac));

            if (existingItem != null && imageId == null)
            {
                existingItem.quantity += itemQuantity;
                success = true;
                message = "Thêm thành công";
            }
            else if (existingItem != null && existingItem.mausac == product.ProductImages.FirstOrDefault(pi => pi.imageId == imageId)?.MauSac)
            {
                existingItem.quantity += itemQuantity;
                success = true;
                message = "Thêm thành công";
            }
            else
            {
                var selectedImage = product.ProductImages.FirstOrDefault(pi => pi.imageId == imageId);
                decimal price = product.ProductImages.FirstOrDefault(pi => pi.imageId == imageId)?.price ?? product.price;
                string image = product.ProductImages.FirstOrDefault(pi => pi.imageId == imageId)?.imageUrl ?? product.image;
                cart.Add(new CartItem
                {
                    Id = product.productId,
                    Name = product.nameProduct,
                    price = price,
                    image = image,
                    quantity = itemQuantity,
                    mausac = selectedImage?.MauSac, // Color
                    ImageId = imageId ?? -1,
                    gift = 0
                });

                success = true;
                message = "Thêm thành công";
            }

            Session["Cart"] = cart;
            int distinctProductCount = cart.Count;

            return Json(new { success, message, count = distinctProductCount });
        }

        public JsonResult addWishList(int productid)
        {
            bool isSuccess = true;
            string message = "Thao tác thành công";
            if (Session["user"] == null)
            {
                return Json(new { isSuccess = false, message = "Vui lòng đăng nhập để thêm sản phẩm yêu thích" });
            }
            var user = Session["user"] as dynamic;
            var userid = (int)user.Id;
            var checkWish = db.Wishlists.FirstOrDefault(item => item.productId == productid && item.userId == userid);
            if (checkWish != null)
            {
                return Json(new { isWarning = true, message = "Sản phẩm đã có trong danh sách yêu thích" });
            }
            Wishlist wishlist = new Wishlist
            {
                productId = productid,
                userId = userid
            };

            db.Wishlists.Add(wishlist);

            // Save changes to the database
            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = "Có lỗi xảy ra khi thêm sản phẩm vào danh sách yêu thích: " + ex.Message;
            }

            return Json(new { isSuccess, message });
        }

        [HttpPost]
        public JsonResult updateCart(int productid, int quantity, string action , int?imageid)
        {

            var sp = db.Products
                .Include(p => p.ProductImages)
                .FirstOrDefault(p => p.productId == productid &&
                                     (imageid == null || p.ProductImages.Any(pi => pi.imageId == imageid)));

            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            var exitsItem = cart.FirstOrDefault(item => item.Id == productid && item.ImageId == imageid);

            if (exitsItem != null)
            {
                exitsItem.quantity = quantity;
            }
            decimal price = imageid == -1
            ? exitsItem.price
            : sp.ProductImages.FirstOrDefault(pi => pi.imageId == imageid)?.price ?? sp.price;

            var newPrice = price * quantity;

            // Tính tổng giá mới cho toàn bộ giỏ hàng
            decimal totalPrice = (decimal)cart.Sum(item =>
                item.quantity * (
                    imageid != -1
                        ? db.Products.Find(item.Id)?.ProductImages.FirstOrDefault(pi => pi.imageId == imageid)?.price
                        : db.Products.Find(item.Id)?.price
                    ?? 0
                )
            );
            Session["Cart"] = cart;

            return Json(new
            {
                success = true,
                message = "Cập nhật giỏ hàng thành công.",
                newQuantity = exitsItem?.quantity ?? quantity,
                newPrice = newPrice,
                totalPrice = totalPrice,
                imageid = imageid
            });
        }

        [HttpPost]
        public JsonResult deleteCart(int productid, int? imageId)
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            // Tìm sản phẩm cần xóa dựa vào productid và imageId (hoặc mausac)
            var exitsItem = cart.FirstOrDefault(item =>
                item.Id == productid &&
                (imageId == null || item.ImageId == imageId));


            // Các biến để tính tổng giá và kiểm tra quà tặng
            decimal totalPrice = 0;
            bool checkGift = false;
            bool checkIssetGift = false;

            if (exitsItem != null)
            {
                // Kiểm tra xem sản phẩm có phải là quà tặng không
                if (exitsItem.gift == 1)
                {
                    Session["gift"] = null;
                    checkGift = true;
                }

                // Xóa sản phẩm khỏi giỏ hàng
                cart.Remove(exitsItem);
            }

            // Tính tổng giá sau khi xóa
            foreach (var item in cart)
            {
                totalPrice += item.price * item.quantity;
            }

            // Kiểm tra nếu tổng giá trị nhỏ hơn 3,000,000 thì xóa quà tặng (nếu có)
            var exitsItemGift = cart.FirstOrDefault(item => item.gift == 1);
            if (totalPrice < 3000000 && exitsItemGift != null)
            {
                cart.Remove(exitsItemGift);
                Session["gift"] = null;
                checkIssetGift = true;
            }

            // Cập nhật giỏ hàng trong session
            Session["Cart"] = cart;
            int distinctProductCount = cart.Count;

            return Json(new
            {
                success = true,
                message = "Xóa thành công.",
                totalPrice = totalPrice,
                count = distinctProductCount,
                checkGift = checkGift,
                checkIssetGift = checkIssetGift,
                image = imageId
            });
        }

        [HttpPost]
        public JsonResult deleteAllCart()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            decimal totalPrice = 0;
            cart.Clear();
            Session["Cart"] = cart;
            foreach (var item in cart)
            {
                totalPrice += item.price;
            }
            int distinctProductCount = cart.Count();
            Session["gift"] = null;
            return Json(new
            {
                success = true,
                message = "Xóa thành công.",
                totalPrice = totalPrice,
                count = distinctProductCount
            });
        }
        [HttpPost]
        public JsonResult addToCartDetails(int productId , int mausac)
        {


            return Json(new {});
        }
    }
}