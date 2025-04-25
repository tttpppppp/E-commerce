using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class PhiVanChuyenController : Controller
    {
        // GET: Admin/PhiVanChuyen
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            var listTranport = db.Transports.ToList();
            return View(listTranport);
        }
        public ActionResult themVanChuyen()
        {
            return View();
        }
        [HttpPost]
        public JsonResult themVanChuyen(Transport transport)
        {
            db.Transports.Add(transport);
            db.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        public JsonResult getPriceVanChuyen(Transport transport)
        {
            decimal price;

            var findVanChuyen = db.Transports.FirstOrDefault(
                item => item.City == transport.City
                && item.District == transport.District
                && item.Ward == transport.Ward
            );

            if (findVanChuyen == null)
            {
                price = 50000;
            }
            else
            {
                price = findVanChuyen.Price;
            }

            HttpCookie priceCookie = new HttpCookie("transportPrice", price.ToString())
            {
                Expires = DateTime.Now.AddMinutes(30) 
            };
            Response.Cookies.Add(priceCookie);
            return Json(new { success = true, price = price });
        }
        public ActionResult deleteVanChuyen(int id)
        {
            var vanchuyen = db.Transports.Find(id);
            if (vanchuyen == null)
            {
                TempData["error"] = "Phí vận chuyển không tồn tại";
                return RedirectToAction("Index");
            }
            try
            {
                db.Transports.Remove(vanchuyen);
                db.SaveChanges();  
                TempData["success"] = "Xóa phí vận chuyển thành công";
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                TempData["error"] = "Đã xảy ra lỗi khi xóa phí vận chuyển: " + ex.Message;
            }
            return RedirectToAction("Index");
        }

    }
}