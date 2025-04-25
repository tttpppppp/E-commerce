using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSale.Models
{
    public class CartController : Controller
    {
        // GET: Cart(
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            mapCategories mapCategories = new mapCategories();
            ViewBag.listCategory = mapCategories.getCategories();
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            decimal total = 0;
            if(cart != null && cart.Count < 0)
            {
                ViewBag.waring = "Không có sản phẩm nào";
            }
            if (cart != null && cart.Count > 0) {
                foreach (var cartItem in cart)
                {
                    total += cartItem.price;
                }
            }
            ViewBag.total = total;
            ViewBag.gift = db.Products.Where(item => item.isGift == 1);
            return View(cart);
        }
    }
}