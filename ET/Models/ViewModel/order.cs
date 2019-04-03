using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ET.Models.ViewModel
{
    public class orderDetail
    {
        public int Id { get; set; }
        public int orderId { get; set; }
        public user user { get; set; }
        public product product { get; set; }
        public int quantity { get; set; }

    }
}