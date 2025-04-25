using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class mapType
    {
        public List<Type> getType()
        {
            WebBanHangASPEntities3 db = new WebBanHangASPEntities3();
            return db.Types.ToList();
        }
    }
}
