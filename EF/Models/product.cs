//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EF.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public product()
        {
            this.images = new HashSet<image>();
        }
    
        public int productId { get; set; }
        public string productName { get; set; }
        public Nullable<int> categoryId { get; set; }
        public Nullable<int> stock { get; set; }
        public Nullable<decimal> price { get; set; }
        public string productDesc { get; set; }
        public Nullable<decimal> discount { get; set; }
        public string image { get; set; }
        public Nullable<int> brandId { get; set; }
    
        public virtual category category { get; set; }
        public virtual brand brand { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<image> images { get; set; }
    }
}
