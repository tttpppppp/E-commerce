using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Admin/Dashboard
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public ActionResult Index()
        {
            if(Session["admin"] == null)
            {
                return Redirect("/Admin/Login");
            }
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var totalRevenueCurrentMonth = db.Orders
            .Where(o => o.created_at.HasValue &&
                        o.created_at.Value.Month == currentMonth &&
                        o.created_at.Value.Year == currentYear)
            .Sum(o => (decimal?)o.total) ?? 0;

            var totalRevenueCurrentYear = db.Orders
               .Where(o => o.created_at.HasValue &&
                           o.created_at.Value.Year == currentYear)
               .Sum(o => (decimal?)o.total) ?? 0;

            var totalRevenueByMonth = db.Orders
                    .GroupBy(o => new
                    {
                        Year = o.created_at.Value.Year,
                        Month = o.created_at.Value.Month
                    })
                    .Select(g => new RevenueData
                    {
                        Year = g.Key.Year,
                        Month = g.Key.Month,
                        TotalRevenue = g.Sum(o => o.total)
                    })
                    .OrderBy(result => result.Year)
                    .ThenBy(result => result.Month)
                    .ToList();

            var countPending = db.Orders.Where(item => item.trangthai == 0).Count();

            var takeOrder = db.Orders.OrderByDescending(item => item.created_at).Take(3);

            ViewBag.TotalRevenueByMonth = totalRevenueByMonth;
            ViewBag.total = totalRevenueCurrentMonth;
            ViewBag.totalYear = totalRevenueCurrentYear;
            ViewBag.countPending = countPending;
            ViewBag.takeOrder = takeOrder;
            return View();
        }

    }
}