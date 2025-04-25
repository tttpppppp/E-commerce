using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Controllers
{
    public class DanhGiaController : Controller
    {
        // GET: DanhGia
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        [HttpPost]
        public JsonResult Index(Review review)
        {
            bool isSuccess = true; // Assume success by default

            try
            {
                if (Session["user"] == null)
                {
                    return Json(new { isSuccess = false, message = "Bạn cần phải đăng nhập để đánh giá" });
                }
                var user = Session["user"] as dynamic;
                var userId = user?.Id;
                if (userId == null)
                {
                    return Json(new { isSuccess = false, message = "User ID not found" });
                }

                review.userid = userId;

                db.Reviews.Add(review);
                db.SaveChanges(); 
                return Json(new { isSuccess = true, message = "Review added successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "An error occurred: " + ex.Message });
            }
        }

    }
}