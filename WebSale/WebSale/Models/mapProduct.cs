using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class mapProduct
    {
        WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
       public List<getListProduct_Result> getAllProduct()
        {
            return db.getListProduct().ToList();
        }
    }
}