using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ET.Models.ViewModel
{
    
    public class product2Image
    {
        public images image { get; set; }
        public product products { get; set; }
        public List<images> imageList { get; set; }
        public List<product> productList { get; set; }
        public string brand { get; set; }

    }
}