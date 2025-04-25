using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSale.Areas.Admin.Controllers
{
    public class PageNotFoundController : Controller
    {
        // GET: Admin/PageNotFound
        public ActionResult Index()
        {
            return View();
        }
    }
}