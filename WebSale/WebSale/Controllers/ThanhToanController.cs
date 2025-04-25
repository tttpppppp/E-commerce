using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using WebSale.Repository;

namespace WebSale.Controllers
{
    public class ThanhToanController : Controller
    {
        private const string Endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        private const string PartnerCode = "MOMOBKUN20180529";
        private const string AccessKey = "klm05TvNBzhg7h7j";
        private const string SecretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";


        private WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        private EmailSender _emailSender = new EmailSender();

        public ActionResult Index()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (Session["user"] != null)
            {
                var user = Session["user"] as dynamic;
                var userId = user.Id;
                ViewBag.user = db.Users.Find(userId);
            }
            decimal total = 0;
            if (cart == null || cart.Count == 0)
            {
                ViewBag.warning = "Không có sản phẩm nào";
            }
            else
            {
                foreach (var cartItem in cart)
                {
                    total += cartItem.price;
                }
            }

            ViewBag.total = total;
            return View(cart);
        }

        [HttpPost]
        public async Task<ActionResult> ThanhToan(string phuongthuc, Shipping shipping)
        {
            try
            {
                // 1. Check if the cart is empty
                var cart = Session["cart"] as List<CartItem>;
                if (cart == null || cart.Count == 0)
                {
                    return Json(new { isSuccess = false, error = "Chưa có sản phẩm vui lòng mua hàng!" });
                }

                // 2. Check if user is logged in
                var user = Session["user"] as dynamic;
                if (user == null)
                {
                    return Json(new { isSuccess = false, error = "Bạn cần đăng nhập để thanh toán!" });
                }

                // 3. Check if transport price is available
                if (Request.Cookies["transportPrice"] == null)
                {
                    return Json(new { isSuccess = false, error = "Bạn chưa tính giá vận chuyển!" });
                }

                // 4. Validate shipping details
                if (string.IsNullOrEmpty(shipping.email))
                {
                    return Json(new { isSuccess = false, error = "Vui lòng nhập email" });
                }
                if (string.IsNullOrEmpty(shipping.phoneNumber))
                {
                    return Json(new { isSuccess = false, error = "Vui lòng nhập mobile" });
                }
                if (string.IsNullOrEmpty(shipping.Address))
                {
                    return Json(new { isSuccess = false, error = "Vui lòng nhập địa chỉ" });
                }

                // 5. Handle payment method
                decimal total = cart.Sum(c => c.price * c.quantity);
                decimal transportPrice = decimal.TryParse(Request.Cookies["transportPrice"]?.Value, out decimal price) ? price : 0;
                total += transportPrice;

                if (phuongthuc == "COD")
                {
                    // Handle Cash on Delivery (COD)
                    await ThanhToanCod(shipping); // Ensure this method handles COD correctly
                    return Json(new { isSuccess = true });
                }
                else if (phuongthuc == "MOMO")
                {
                    // Handle MoMo Payment
                    // 5.1 Add shipping to database
                    shipping.PhuongThuc = "Thanh toán qua MoMo";
                    db.Shippings.Add(shipping);
                    db.SaveChanges();

                    int orderId = new Random().Next(1000, 10000); // Random orderId
                    var order = new Order
                    {
                        orderId = orderId,
                        userId = user.Id,
                        shippingID = shipping.shippingID,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                        total = total,
                        giaVanChuyen = transportPrice,
                        trangthai = 0 // Initial status
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    // 5.2 Add order details to database
                    var orderDetails = cart.Select(cartItem => new OrderDetail
                    {
                        orderId = order.orderId,
                        productId = cartItem.Id,
                        quantity = cartItem.quantity,
                        price_at_order = cartItem.price,
                        mausac = cartItem.mausac,
                    }).ToList();

                    db.OrderDetails.AddRange(orderDetails);

                    // 5.3 Update product stock and sales quantity
                    foreach (var cartItem in cart)
                    {
                        var product = db.Products.FirstOrDefault(p => p.productId == cartItem.Id);
                        if (product == null || product.quantity < cartItem.quantity)
                        {
                            return Json(new { isSuccess = false, error = "Không đủ số lượng trong kho." });
                        }

                        product.quantity -= cartItem.quantity;
                        product.sold_quantity = (product.sold_quantity ?? 0) + cartItem.quantity;
                    }

                    db.SaveChanges();

                    // 5.4 Send email confirmation
                    string subject = "Xác Nhận Đơn Hàng";
                    string message = $"<h1>Cảm ơn bạn đã đặt hàng!</h1><p>Mã đơn hàng của bạn là: <strong>{order.orderId}</strong></p>";
                    message += $"<p>Tổng giá trị: <strong>{total + transportPrice:C}</strong></p>";
                    message += "<table style='width:100%; border-collapse: collapse;'>";
                    message += "<tr><th style='border: 1px solid #dddddd; padding: 8px;'>Tên sản phẩm</th><th style='border: 1px solid #dddddd; padding: 8px;'>Hình ảnh</th><th style='border: 1px solid #dddddd; padding: 8px;'>Số lượng</th><th style='border: 1px solid #dddddd; padding: 8px;'>Giá</th></tr>";

                    foreach (var product in cart)
                    {
                        message += $"<tr><td style='border: 1px solid #dddddd; padding: 8px;'>{product.Name}</td><td style='border: 1px solid #dddddd; padding: 8px;'><img src='https://localhost:44338/{product.image}' alt='{product.Name}' style='width: 100px; height: auto;' /></td><td style='border: 1px solid #dddddd; padding: 8px;'>{product.quantity}</td><td style='border: 1px solid #dddddd; padding: 8px;'>{product.price:C}</td></tr>";
                    }

                    message += "</table><p>Cảm ơn bạn đã đặt hàng!</p>";
                    await _emailSender.SendEmailAsync(shipping.email, subject, message);

                    // 5.5 Clear session and cookies
                    Session["cart"] = null;
                    Session["coupon"] = null;
                    Session["gift"] = null;
                    if (Request.Cookies["transportPrice"] != null)
                    {
                        var cookie = new HttpCookie("transportPrice") { Expires = DateTime.Now.AddDays(-1) };
                        Response.Cookies.Add(cookie);
                    }

                    // 5.6 Initiate MoMo payment (Mock URL for now)
                    string orderInfo = "Thanh toán qua MoMo";
                    string amount = total.ToString("0"); // Calculate dynamically from cart
                    string orderIdStr = orderId.ToString();
                    string redirectUrl = "https://localhost:44338/thanks";
                    string ipnUrl = "https://localhost:44338/thanks";

                    string requestId = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString();
                    string requestType = "payWithATM"; // Payment method

                    // Generate the signature for MoMo
                    string rawHash = $"accessKey={AccessKey}&amount={amount}&extraData=&ipnUrl={ipnUrl}&orderId={orderIdStr}&orderInfo={orderInfo}&partnerCode={PartnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
                    string signature;
                    using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretKey)))
                    {
                        signature = BitConverter.ToString(hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash))).Replace("-", "").ToLower();
                    }

                    // Prepare the MoMo request data
                    var paymentData = new
                    {
                        partnerCode = PartnerCode,
                        partnerName = "Test",
                        storeId = "MomoTestStore",
                        requestId = requestId,
                        amount = amount,
                        orderId = orderIdStr,
                        orderInfo = orderInfo,
                        redirectUrl = redirectUrl,
                        ipnUrl = ipnUrl,
                        lang = "vi",
                        extraData = "",
                        requestType = requestType,
                        signature = signature
                    };

                    using (var client = new HttpClient())
                    {
                        var jsonContent = new StringContent(JsonConvert.SerializeObject(paymentData), Encoding.UTF8, "application/json");
                        var response = await client.PostAsync(Endpoint, jsonContent);

                        if (!response.IsSuccessStatusCode)
                        {
                            return Json(new { isSuccess = false, error = "Lỗi trong quá trình xử lý thanh toán." });
                        }

                        var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());
                        string paymentUrl = result.payUrl.ToString();
                        return Json(new { isSuccess = true, paymentUrl = paymentUrl });
                    }
                }

                // 6. Return error if payment method is invalid
                return Json(new { isSuccess = false, error = "Phương thức thanh toán không hợp lệ!" });
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return Json(new { isSuccess = false, error = $"Đã xảy ra lỗi: {ex.Message}" });
            }
        }
        [HttpPost]
        public async Task<JsonResult> ThanhToanCod(Shipping shipping)
        {
            var cart = Session["cart"] as List<CartItem>;
            var user = Session["user"] as dynamic;

            decimal total = cart.Where(cartItem => cartItem.gift == 0).Sum(cartItem => cartItem.price * cartItem.quantity);
            if (Session["coupon"] != null)
            {
                total -= (decimal)Session["coupon"];
            }
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    shipping.PhuongThuc = "Thanh toán qua COD (Giao hàng trực tiếp)";
                    db.Shippings.Add(shipping);
                    db.SaveChanges();

                    int orderId = new Random().Next(1000, 10000);

                    decimal transportPrice = 0;
                    var priceCookie = Request.Cookies["transportPrice"];

                    if (priceCookie != null && decimal.TryParse(priceCookie.Value, out decimal price))
                    {
                        transportPrice = price;
                        total += price;
                    }
                    else
                    {
                        return Json(new { error = "Phí vận chuyển không hợp lệ!" });
                    }

                    var order = new Order
                    {
                        orderId = orderId,
                        userId = user.Id,
                        shippingID = shipping.shippingID,
                        created_at = DateTime.Now,
                        updated_at = DateTime.Now,
                        total = total,
                        giaVanChuyen = transportPrice,
                        trangthai = 0
                    };
                    db.Orders.Add(order);
                    db.SaveChanges();

                    // Save Order Details
                    var orderDetails = cart.Select(cartItem => new OrderDetail
                    {
                        orderId = order.orderId,
                        productId = cartItem.Id,
                        quantity = cartItem.quantity,
                        price_at_order = cartItem.price,
                        mausac = cartItem.mausac,

                    }).ToList();

                    db.OrderDetails.AddRange(orderDetails);

                    foreach (var cartItem in cart)
                    {
                        var product = db.Products.FirstOrDefault(p => p.productId == cartItem.Id);
                        if (product != null)
                        {
                            if (product.quantity < cartItem.quantity)
                            {
                                throw new Exception("Không đủ số lượng trong kho.");
                            }
                            product.quantity -= cartItem.quantity;
                            product.sold_quantity += cartItem.quantity;
                        }
                        db.SaveChanges();
                    }
                    db.SaveChanges();
                    transaction.Commit();

                    string subject = "Xác Nhận Đơn Hàng";
                    string message = "<h1>Cảm ơn bạn đã đặt hàng!</h1>";
                    message += $"<p>Mã đơn hàng của bạn là: <strong>{order.orderId}</strong></p>";
                    message += $"<p>Tổng giá trị: <strong>{total + transportPrice:C}</strong></p>";
                    message += "<table style='width:100%; border-collapse: collapse;'>";
                    message += "<tr><th style='border: 1px solid #dddddd; padding: 8px;'>Tên sản phẩm</th><th style='border: 1px solid #dddddd; padding: 8px;'>Hình ảnh</th><th style='border: 1px solid #dddddd; padding: 8px;'>Số lượng</th><th style='border: 1px solid #dddddd; padding: 8px;'>Giá</th></tr>";

                    foreach (var product in cart)
                    {
                        message += "<tr>";
                        message += $"<td style='border: 1px solid #dddddd; padding: 8px;'>{product.Name}</td>";
                        message += $"<td style='border: 1px solid #dddddd; padding: 8px;'><img src='https://localhost:44338/{product.image}' alt='{product.Name}' style='width: 100px; height: auto;'/></td>";
                        message += $"<td style='border: 1px solid #dddddd; padding: 8px;'>{product.quantity}</td>";
                        message += $"<td style='border: 1px solid #dddddd; padding: 8px;'>{product.price:C}</td>";
                        message += "</tr>";
                    }

                    message += "</table>";
                    message += "<p>Cảm ơn bạn đã đặt hàng!</p>";

                    await _emailSender.SendEmailAsync(shipping.email, subject, message);

                    Session["cart"] = null;
                    Session["coupon"] = null;
                    Session["gift"] = null;
                    if (Request.Cookies["transportPrice"] != null)
                    {
                        var cookie = new HttpCookie("transportPrice");
                        cookie.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(cookie);
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { isSuccess = false, error = ex.Message });
                }
            }
            return Json(new { isSuccess = true });
        }
    }
}
