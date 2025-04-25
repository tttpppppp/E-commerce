using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;
using WebSale.Models;

namespace WebSale.Controllers
{
    public class CouponController : Controller
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3 ();
        [HttpPost]
        public JsonResult Index(string coupon)
        {
            List<CartItem> cart = Session["cart"] as List<CartItem> ?? new List<CartItem>();

            bool isSuccess = true;
            var total = cart.Sum(item => item.price * item.quantity); 
            var listCoupon = db.Coupons.ToList();
            var priceCookie = Request.Cookies["transportPrice"];
            var message = "";
            if (Session["cart"] == null || cart.Count == 0)
            {
                return Json(new { isSuccess = false, message = "Vui lòng mua hàng để được giảm giá" });
            }
            if (priceCookie == null)
            {
                return Json(new { isSuccess = false, message = "Vui lòng tính phí vận chuyển" });
            }
            if (Session["coupon"] == null)
            {
                var appliedCoupon = listCoupon.FirstOrDefault(c => c.name == coupon);

                if (appliedCoupon != null)
                {
                    if (appliedCoupon.dateExpier < DateTime.Now)
                    {
                        return Json(new { isSuccess = false, message = "Mã giảm giá đã hết hạn" });
                    }
                    else
                    {
                        // Apply the discount
                        total -= appliedCoupon.price;
                        message = $"Bạn đã được giảm {appliedCoupon.price.ToString("C", new System.Globalization.CultureInfo("vi-VN"))}";
                        Session["coupon"] = appliedCoupon.price; 
                    }
                }
                else
                {
                    message = "Mã của bạn không đúng!";
                    isSuccess = false;
                }
            }
            else
            {
                message = "Bạn đã được giảm!";
            }
            return Json(new { total, message, isSuccess });
        }
    }
}