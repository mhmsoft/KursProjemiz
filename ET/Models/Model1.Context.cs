﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class MembershipEntities : DbContext
    {
        public MembershipEntities()
            : base("name=MembershipEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<brand> brand { get; set; }
        public virtual DbSet<category> category { get; set; }
        public virtual DbSet<member> member { get; set; }
        public virtual DbSet<product> product { get; set; }
        public virtual DbSet<images> images { get; set; }
        public virtual DbSet<address> address { get; set; }
        public virtual DbSet<city> city { get; set; }
        public virtual DbSet<district> district { get; set; }
        public virtual DbSet<user> user { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<userToaddress> userToaddress { get; set; }
        public virtual DbSet<wishlist> wishlist { get; set; }
    }
}