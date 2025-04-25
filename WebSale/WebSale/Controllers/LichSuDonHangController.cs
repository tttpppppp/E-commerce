using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;
using System.Web.Http.Results;
namespace WebSale.Controllers
{
    public class LichSuDonHangController : Controller
    {
        // GET: LichSuDonHang
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            if (Session["user"] == null)
            {
                return Redirect("/khongtimthaytrang");
            }
            var user = Session["user"] as dynamic;
            var userid = (int)user.Id;
            var takhoan = db.Users.FirstOrDefault(item => item.userId == userid);

            var listorder = db.Orders.Where(item => item.userId == userid)
                        .OrderByDescending(item => item.orderId).Take(4)
                        .Include(o => o.Shipping)
                        .ToList();

            var listordercount = db.Orders.Where(item => item.userId == userid)
                      .OrderByDescending(item => item.orderId)
                      .Include(o => o.Shipping)
                      .ToList();

            ViewBag.listorder = listorder;
            ViewBag.listordercount = listordercount;
            return View(takhoan);
        }
        public JsonResult LoadMoreOrders(int skip)
        {
            if (Session["user"] == null)
            {
                return Json(new { success = false, message = "User not logged in." }, JsonRequestBehavior.AllowGet);
            }

            var user = Session["user"] as dynamic;
            var userid = (int)user.Id;
            int take = 4;

            var listorder = db.Orders
                .Where(o => o.userId == userid)
                .OrderByDescending(o => o.orderId)
                .Skip(skip)
                .Take(take)
                .Include(o => o.Shipping)
                .ToList();

            if (!listorder.Any())
            {
                return Json(new { success = false, message = "No more orders." }, JsonRequestBehavior.AllowGet);
            }

            // Trả về dữ liệu JSON của danh sách đơn hàng
            var result = listorder.Select(item => new
            {
                orderId = item.orderId,
                status = item.trangthai == 0 ? "Chưa xử lý" :
                         item.trangthai == 1 ? "Đang giao" :
                         item.trangthai == 2 ? "Đã giao hàng" : "Đơn bị hủy",
                price = item.total.ToString("N0") + "₫",
                address = item.Shipping != null && !string.IsNullOrEmpty(item.Shipping.Address)
                            ? item.Shipping.Address
                            : "Không có địa chỉ",
                ward = item.Shipping.ward,
                district = item.Shipping.district,
                createdAt = item.created_at
            }).ToList();

            return Json(new { success = true, orders = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult huyDonUser(int orderid)
        {
            var order = db.Orders.FirstOrDefault(item => item.orderId == orderid);
            order.trangthai = 3;
            db.SaveChanges();
            return Json(new { isSuccess = true});
        }

    }

}