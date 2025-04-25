using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSale.Models
{
    public class BlogModel
    {
        public Blog BlogDefault { get; set; }
        public List<Blog> BlogList { get; set; }
    }
}