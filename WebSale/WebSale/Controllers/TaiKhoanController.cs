using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;
using System.Web.Helpers;
using System.Data.Entity.Validation;
namespace WebSale.Controllers
{
    public class TaiKhoanController : Controller
    {
        // GET: TaiKhoan
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            if (Session["user"] == null) {
                return Redirect("/khongtimthaytrang");
            }
            var user = Session["user"] as dynamic;
            var userid = (int)user.Id;
            var takhoan = db.Users.FirstOrDefault(item => item.userId == userid);
            var listorder = db.Orders.Where(item => item.userId == userid).Include(o => o.Shipping).ToList();

            ViewBag.listorder = listorder;
            return View(takhoan);
        }
        [AllowAnonymous]
        public JsonResult updateUser(string email, string firstname, string name, string mobile, string address)
        {
            bool isSuccess = true;

            // Kiểm tra nếu session không có user
            var users = Session["user"] as dynamic;
            if (users == null)
            {
                return Json(new { isSuccess = false });
            }

            var userid = (int)users.Id;

            // Tìm người dùng trong cơ sở dữ liệu
            var userupdate = db.Users.FirstOrDefault(item => item.userId == userid);
            if (userupdate == null)
            {
                return Json(new { isSuccess = false, message = "User not found" });
            }

            // Cập nhật các trường nếu giá trị truyền vào không phải null, nếu null thì giữ giá trị hiện tại
            userupdate.email = email ?? userupdate.email;
            userupdate.firstName = firstname ?? userupdate.firstName;
            userupdate.name = name ?? userupdate.name;
            userupdate.mobile = mobile ?? userupdate.mobile;
            userupdate.address = address ?? userupdate.address;
            db.Entry(userupdate).State = EntityState.Modified;
            // Lưu thay đổi vào cơ sở dữ liệu
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                // Log các lỗi validation chi tiết
                var errorMessages = e.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors)
                    .Select(validationError => validationError.ErrorMessage)
                    .ToList();

                // Trả về lỗi chi tiết
                return Json(new { isSuccess = false, errors = errorMessages });
            }

            return Json(new { isSuccess = true });
        }

    }
}