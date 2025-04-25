using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSale.Models
{
    public class mapCategories
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public List<Category> getCategories()
        {
            
            return db.Categories.ToList();

        }
        public List<Category> getCategoriesMenu()
        {
            return db.Categories.Take(8).OrderBy(item => item.categoryId).ToList();
        }
        public List<Category> getCategoriesMenuHot()
        {
            return db.Categories.OrderBy(p => p.categoryId).Skip(8).ToList();
        }
    }
}