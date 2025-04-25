using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime? CreatedAt { get; set; } // Nullable DateTime if required
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string ShippingEmail { get; set; }
        public string PhoneNumber { get; set; }
        public decimal TotalPrice { get; set; }
        public int TrangThai { get; set; }
        public string city { get; set; }
        public string didistrict { get; set; }
        public string ward { get; set; }
        public decimal giaVanChuyen { get; set; }
    }

}