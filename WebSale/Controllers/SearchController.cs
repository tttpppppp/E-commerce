using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSale.Models;

namespace WebSale.Controllers
{
    public class SearchController : Controller
    {
        private WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        // GET: Serach
        public ActionResult Index(string keyword , int page , int pageSize = 2)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                ViewBag.warning = "Bạn cần nhập từ khóa tìm kiếm";
                return View();
            }
            keyword = keyword.Trim().ToLower();

            HttpCookie keywordCookie = Request.Cookies["searchKeywords"];
            List<string> keywordList = new List<string>();

            if (keywordCookie != null)
            {

                keywordList = keywordCookie.Value.Split(',').ToList();
            }
            keywordList.Add(keyword);


            string keywordsString = string.Join(",", keywordList);
            keywordCookie = new HttpCookie("searchKeywords");
            keywordCookie.Value = keywordsString;
            keywordCookie.Expires = DateTime.Now.AddDays(365); 
            Response.Cookies.Add(keywordCookie);

            var searchResults = db.Products
                .Where(p => p.nameProduct.ToLower().Contains(keyword.ToLower()) || p.description.Contains(keyword.ToLower()))
                .ToList();
            var count = searchResults.Count;
            var totalResults = searchResults.Count();

            var totalPages = (int)Math.Ceiling((double)totalResults / pageSize);

            var paginatedResults = searchResults
              .OrderBy(p => p.nameProduct) 
              .Skip((page - 1) * pageSize)
              .Take(pageSize)
              .ToList();

            if (Request.IsAjaxRequest())
            {
                return PartialView("_ProductList", paginatedResults); 
            }

            ViewBag.Count = count;
            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            return View(paginatedResults);
        }
    }
}