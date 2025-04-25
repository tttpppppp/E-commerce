using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class ChiTietDonHangController : Controller
    {
        // GET: Admin/ChiTietDonHang
        public ActionResult Index(int orderId)
        {
            if (Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            using (var db = new WebBanHangASPEntities3())
            {
                var query = db.Orders
                  .Where(order => order.orderId == orderId)
                  .SelectMany(order => order.OrderDetails)
                  .Select(orderDetail => new OrderDetailViewModel
                  {
                      OrderId = orderDetail.orderId,
                      NameProduct = orderDetail.Product.nameProduct,
                      Quantity = orderDetail.quantity,
                      PriceAtOrder = orderDetail.price_at_order,
                      Image = orderDetail.Product.image,

                  })
                  .ToList();

                //var query = from order in db.Orders
                //            join orderDetail in db.OrderDetails on order.orderId equals orderDetail.orderId
                //            join product in db.Products on orderDetail.productId equals product.productId
                //            where order.orderId == orderId
                //            select new OrderDetailViewModel
                //            {
                //                OrderId = order.orderId,
                //                Quantity = orderDetail.quantity,
                //                PriceAtOrder = orderDetail.price_at_order,
                //                NameProduct = product.nameProduct,
                //                Image = product.image
                //            };
                var resultList = query.ToList();
                return View(resultList);
            }
        }
    }
}