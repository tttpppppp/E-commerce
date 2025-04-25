using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class ProductImageViewModel
    {
        public int ImageId { get; set; }
        public string image {  get; set; }
        public decimal Price { get; set; }
        public decimal Sale { get; set; }
        public string MauSac { get; set; }
        public bool IsDefault { get; set; }
    }
}