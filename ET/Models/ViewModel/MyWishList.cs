using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ET.Models.ViewModel
{
    public class MyWishList
    {
        public int Id { get; set; }
        public product product { get; set; }
        public user user { get; set; }
    }
}