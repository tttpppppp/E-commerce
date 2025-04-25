using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;
using System.Data.Entity;
using System.Windows.Forms;
namespace WebSale.Controllers

{
    public class ChiTietController : Controller
    {
        // GET: ChiTiet
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3 ();
        public ActionResult Index(int? orderid)
        {
            if (orderid == null) {
                return Redirect("/khongtimthaytrang");
            }
            ViewBag.Title = "#" + orderid + " - " + "Mew Mobile";
            if (Session["user"] == null)
            {
                return Redirect("/khongtimthaytrang");
            }
            var user = Session["user"] as dynamic;
            var userid = (int)user.Id;

            var takhoan = db.Users.FirstOrDefault(item => item.userId == userid);
            var listorder = db.Orders.Where(item => item.userId == userid).Include(o => o.Shipping).ToList();

            var que = db.Orders
            .Where(item => item.orderId == orderid)
            .Include(s => s.Shipping)
            .Include(orderd => orderd.OrderDetails.Select(od => od.Product)) 
            .ToList();

            ViewBag.details = que;
            ViewBag.listorder = listorder;

            return View(takhoan);
        }
    }
}