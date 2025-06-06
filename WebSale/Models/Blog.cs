//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebSale.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Blog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Blog()
        {
            this.MucLucs = new HashSet<MucLuc>();
        }
    
        public int id { get; set; }
        public string tieu_de { get; set; }
        public string dang_boi { get; set; }
        public Nullable<System.DateTime> ngay_dang { get; set; }
        public string content { get; set; }
        public Nullable<int> danh_muc_id { get; set; }
        public string slug { get; set; }
        public Nullable<int> status { get; set; }
        public string image_thumb { get; set; }
    
        public virtual DanhMucBlog DanhMucBlog { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MucLuc> MucLucs { get; set; }
    }
}
