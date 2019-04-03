//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ET.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class address
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public address()
        {
            this.user = new HashSet<user>();
            this.userToaddress = new HashSet<userToaddress>();
        }
    
        public int addressId { get; set; }
        public string title { get; set; }
        public string addressName { get; set; }
        public Nullable<int> cityId { get; set; }
        public Nullable<int> districtId { get; set; }
    
        public virtual city city { get; set; }
        public virtual district district { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<user> user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<userToaddress> userToaddress { get; set; }
    }
}