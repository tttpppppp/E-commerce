using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class ProductViewModel
    {
        
            public int productId { get; set; }
            public string slug { get; set; }
            public string nameProduct { get; set; }
            public decimal price { get; set; }
            public decimal sale_price { get; set; }
            public decimal percen_sale { get; set; }
            public string categoryName { get; set; }
            public string brandName { get; set; }
            public string typeName { get; set; }
            public string image { get; set; }
            public string slugBrand { get; set; }
            public string slugType { get; set; }
            public int idBrand { get; set; }
            public int idType { get; set; }
            public string des { get; set; }
            public List<ProductImageModel> Images { get; set; }
            public List<ProductImageViewModel> Image { get; set; }
            public int quantity { get; set; }
            public int sold_quantity { get; set; }
            public string masku { get; set; }
            public int idMauSacDefault { get; set; }

    }
}