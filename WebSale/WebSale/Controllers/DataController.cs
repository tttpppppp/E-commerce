using System.Web.Http;
using WebSale.Models;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Web.Helpers;
using StackExchange.Redis;
using System.CodeDom;
namespace WebSale.Controllers
{
    public class DataController : ApiController
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        [Route("api/data/danhsach")]
        [HttpGet]
        public async Task<IHttpActionResult> DanhSach()
        {
            try
            {
                var products = await db.Products.Include(item => item.Brand).ToListAsync();
                //var productDtos = products.Select(p => new ProductViewModel
                ////{
                ////    nameProduct = p.nameProduct,
                ////    brandName = p.Brand.nameBrand
                ////}).ToList();
                var orderDetailsWithInfo = db.Orders
                       .SelectMany(order => order.OrderDetails)
                       .Select(orderDetail => new
                       {
                           OrderId = orderDetail.orderId,
                           ProductName = orderDetail.Product.nameProduct,
                           Quantity = orderDetail.quantity,
                           PriceAtOrder = orderDetail.price_at_order,
                           OrderDate = orderDetail.Order.order_date
                       })
                       .ToList();

                int orderId = 28;
                var orderDetails = db.Orders
                   .Where(order => order.orderId == orderId)
                   .SelectMany(order => order.OrderDetails) 
                   .Select(orderDetail => new
                   {
                       OrderId = orderDetail.orderId,
                       ProductName = orderDetail.Product.nameProduct,
                       Quantity = orderDetail.quantity,
                       PriceAtOrder = orderDetail.price_at_order,
                       OrderDate = orderDetail.Order.order_date
                   })
                   .ToList();

                var orders = db.Orders
                    .Select(order => new
                    {
                        orderId = order.orderId,
                        orderDetails = order.OrderDetails
                    })
                    .ToList();

                return Json(orders);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}
