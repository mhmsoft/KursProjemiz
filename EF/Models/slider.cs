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
    
    public partial class slider
    {
        public int sliderId { get; set; }
        public string caption { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Nullable<decimal> price { get; set; }
        public byte[] imagepath { get; set; }
    }
}
