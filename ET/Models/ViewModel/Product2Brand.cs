using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ET.Models.ViewModel
{
    public class Product2Brand
    {
        int _count;
        public int Count {
            get { return _count; }
            set { _count = value; }
        }
        public string brandName { get; set; }
        public int brandId { get; set; }
    }
}