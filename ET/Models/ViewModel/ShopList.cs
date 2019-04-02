using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ET.Models.ViewModel
{
    public class BasketItem
    {
        public Guid Id { get; set; }
        public product product { get; set; }
        public int quantity { get; set; }
        public images img { get; set; }
        public System.DateTime DateCreated { get; set; }
        public int ProductId { get; set; }

    }

}