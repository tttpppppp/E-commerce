using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Service.VNPAY;

namespace WebSale.Controllers
{
    public class thanksController : Controller
    {
        // GET: thanks
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult responsevnpay(string vnp_Amount, string vnp_BankCode, string vnp_BankTranNo, string vnp_OrderInfo)
        {
            ViewBag.Amount = vnp_Amount;
            ViewBag.Bank = vnp_BankCode;
            ViewBag.BankTransactionNumber = vnp_BankTranNo;
            ViewBag.OrderInfo = vnp_OrderInfo;
            return View();
        }

    }
}