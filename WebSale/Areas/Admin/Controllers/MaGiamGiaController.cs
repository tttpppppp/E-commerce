using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class MaGiamGiaController : Controller
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        // GET: Admin/MaGiamGia
        public ActionResult Index()
        {
            var listMaGiam = db.Coupons.ToList();
            return View(listMaGiam);
        }
        public ActionResult themMaGiamGia()
        {
            return View();
        }
        [HttpPost]
        public ActionResult themMaGiamGia(Coupon coupon) {
            try
            {
                db.Coupons.Add(coupon);
                db.SaveChanges();
                TempData["success"] = "Thêm mã giảm giá thành công";
            }
            catch (Exception ex) {
                TempData["success"] = "Thêm mã giảm giá thất bị";
            }
            return View();
        }
    }
}