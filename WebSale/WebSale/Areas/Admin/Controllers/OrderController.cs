using OfficeOpenXml.Style;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;

namespace WebSale.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        // GET: Admin/Order
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            //var que = db.Orders.Include(s => s.Shipping).Include(u => u.User).ToList();

            var query = from order in db.Orders
                        join user in db.Users on order.userId equals user.userId
                        join shipping in db.Shippings on order.shippingID equals shipping.shippingID
                        select new OrderViewModel
                        {
                            OrderId = order.orderId,
                            CreatedAt = order.created_at,
                            UserName = user.name,
                            UserEmail = user.email,
                            FullName = shipping.fullName,
                            Address = shipping.Address,
                            ShippingEmail = shipping.email,
                            PhoneNumber = shipping.phoneNumber,
                            TotalPrice = order.total,
                            city = shipping.city,
                            didistrict = shipping.district,
                            ward = shipping.ward,
                            giaVanChuyen = (decimal)order.giaVanChuyen,
                            TrangThai = (int)order.trangthai
                        };

            var resultList = query.OrderByDescending(item => item.OrderId).ToList();
            return View(resultList);
        }
        public ActionResult ExportToExcel()
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }

            var query = from order in db.Orders
                        join user in db.Users on order.userId equals user.userId
                        join shipping in db.Shippings on order.shippingID equals shipping.shippingID
                        select new OrderViewModel
                        {
                            OrderId = order.orderId,
                            CreatedAt = order.created_at,
                            UserName = user.name,
                            UserEmail = user.email,
                            FullName = shipping.fullName,
                            Address = shipping.Address,
                            ShippingEmail = shipping.email,
                            PhoneNumber = shipping.phoneNumber,
                            TotalPrice = order.total
                        };

            var resultList = query.ToList();

            // Tạo Excel package
            using (ExcelPackage package = new ExcelPackage())
            {
                // Tạo worksheet mới
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Orders");

                // Thêm tiêu đề cho các cột
                worksheet.Cells[1, 1].Value = "Order ID";
                worksheet.Cells[1, 2].Value = "Created At";
                worksheet.Cells[1, 3].Value = "User Name";
                worksheet.Cells[1, 4].Value = "User Email";
                worksheet.Cells[1, 5].Value = "Full Name";
                worksheet.Cells[1, 6].Value = "Address";
                worksheet.Cells[1, 7].Value = "Shipping Email";
                worksheet.Cells[1, 8].Value = "Phone Number";
                worksheet.Cells[1, 9].Value = "Total Price";

                // Thêm dữ liệu vào Excel
                for (int i = 0; i < resultList.Count; i++)
                {
                    var order = resultList[i];
                    worksheet.Cells[i + 2, 1].Value = order.OrderId;
                    worksheet.Cells[i + 2, 2].Value = order.CreatedAt?.ToString("dd-MM-yyyy");  // Định dạng ngày
                    worksheet.Cells[i + 2, 3].Value = order.UserName;
                    worksheet.Cells[i + 2, 4].Value = order.UserEmail;
                    worksheet.Cells[i + 2, 5].Value = order.FullName;
                    worksheet.Cells[i + 2, 6].Value = order.Address;
                    worksheet.Cells[i + 2, 7].Value = order.ShippingEmail;
                    worksheet.Cells[i + 2, 8].Value = order.PhoneNumber;
                    worksheet.Cells[i + 2, 9].Value = order.TotalPrice;
                }

                // Định dạng tiêu đề
                using (var range = worksheet.Cells[1, 1, 1, 9])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
                }

                // Tự động điều chỉnh kích thước cột
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Lưu và trả về file Excel
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                // Trả về file Excel dưới dạng download
                string excelName = $"Orders_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }
        [HttpPost]
        public JsonResult changeStatusOrder(string status, int orderId)
        {
            var order = db.Orders.Find(orderId);

            if (order == null)
            {
                return Json(new { isSuccess = false, message = "Không tìm thấy đơn hàng." });
            }

            switch (status)
            {
                case "dang_giao":
                    order.trangthai = 1; // Đang giao hàng
                    break;
                case "da_giao":
                    order.trangthai = 2; // Đã giao
                    break;
                case "huy_don":
                    order.trangthai = 3; //Hủy đơn
                    break;
                default:
                    return Json(new { isSuccess = false, message = "Trạng thái không hợp lệ." });
            }
            try
            {
                db.SaveChanges();
                return Json(new { isSuccess = true, message = "Cập nhật trạng thái đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}