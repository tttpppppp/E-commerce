using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Controllers
{
    public class QuaTangController : Controller
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        // GET: QuaTang
        [HttpPost]
        public JsonResult Index(int productId)
        {
            var success = false;
            var message = "Thêm thất bại";
            List<CartItem> cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var product = db.Products.FirstOrDefault(p => p.productId == productId);
            try
            {
                    if (product == null)
                    {
                        message = "Sản phẩm không tồn tại.";
                        return Json(new { success, message });
                    }

                    var existingItem = cart.FirstOrDefault(p => p.Id == productId);

                    if (existingItem != null)
                    {
                        success = false;
                        message = "Sản phẩm chỉ thêm 1 lần!";
                    }
                    else
                    {
                        cart.Add(new CartItem
                        {
                            Id = product.productId,
                            Name = product.nameProduct,
                            price = (decimal)product.price,
                            image = product.image,
                            quantity = 1, 
                            gift = 1           
                        });
                        success = true;
                        message = "Thêm thành công";
                    }
                    Session["Cart"] = cart;
                
            }
            catch (Exception ex)
            {
                message = "Có lỗi xảy ra: " + ex.Message;
            }
            Session["gift"] = productId; 
            int distinctProductCount = cart.Count();
            return Json(new
            {
                success,
                message,
                count = distinctProductCount,
                item = new
                {
                    productId = product.productId,
                    nameProduct = product.nameProduct,
                    price = product.price,
                    image = product.image,

                }
            });

        }
    }
}