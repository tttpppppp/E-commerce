using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Controllers
{
    public class YeuThichController : Controller
    {
        // GET: YeuThich
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                var user = Session["user"] as dynamic;
                var userId = (int)user.Id;
                var listYeuThich = db.Wishlists
                         .Where(item => item.userId == userId)
                         .Include("Product")
                         .ToList();
                return View(listYeuThich);

            }
            else
            {
                ViewBag.User = "Vui lòng đăng nhập để xem danh sách yêu thích.";
            }
            return View();
        }
        [HttpPost]
        public JsonResult removeYeuThich(int id)
        {
            if (Session["user"] == null)
            {
                return Json(new { success = false, message = "User session expired or not logged in." });
            }

            var user = Session["user"] as dynamic;
            var userId = (int)user.Id;

            try
            {
                var wishlist = db.Wishlists.FirstOrDefault(item => item.userId == userId && item.productId == id);

                if (wishlist == null)
                {
                    return Json(new { success = false, message = "Wishlist item not found." ,  user = userId , productid = id });
                }

                db.Wishlists.Remove(wishlist);
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log exception as needed (ex)
                return Json(new { success = false, message = "An error occurred while removing the item." });
            }
        }

    }
}