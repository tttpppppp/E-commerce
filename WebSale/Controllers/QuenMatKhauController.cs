using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using WebSale.Repository;

namespace WebSale.Controllers
{
    public class QuenMatKhauController : Controller
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        private EmailSender _emailSender = new EmailSender();
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Index(User user)
        {
            if (user.email == "")
            {
                TempData["error"] = "Vui lòng nhập email";
                return View();
            }
            var checkemail = db.Users.FirstOrDefault(item => item.email == user.email);

            if (checkemail == null) {
                TempData["error"] = "Email không tồn tại";
                return View();
            }
            {
                string token = Guid.NewGuid().ToString();
                checkemail.token = token;
                db.SaveChanges();
                var reciver = checkemail.email;
                var subject = "Thay đổi mật khẩu cho người dùng" + reciver;

                var resetLink = $"{Request.Url.Scheme}://{Request.Url.Authority}/NewPass?email={checkemail.email}&token={checkemail.token}";
                var message = $"Ấn vào link sau để thay đổi mật khẩu: <a href='{resetLink}'>Thay đổi mật khẩu</a>";

                await _emailSender.SendEmailAsync(reciver, subject, message);
            }
            TempData["success"] = "Đã gửi về email vui lòng kiểm tra email của bạn";
            return View();
        }
    }
}