using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string image { get; set; }
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string mausac {  get; set; }
        public int gift { get; set; }
        public int? ImageId { get; set; }

    }
}