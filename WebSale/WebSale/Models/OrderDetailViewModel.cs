using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtOrder { get; set; }
        public string NameProduct { get; set; }
        public string Image { get; set; }
    }
}