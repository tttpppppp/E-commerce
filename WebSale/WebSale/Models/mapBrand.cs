using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class mapBrand
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
        public List<Brand> getAllBrand()
        {
            return db.Brands.ToList();
        }
    }
}