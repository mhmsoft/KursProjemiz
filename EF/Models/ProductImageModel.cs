using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EF.Models
{
    public class ProductImageModel
    {
        public List<image> Imagelist { get; set; }
        public List<product> Productlist { get; set; }
        public image Image { get; set; }
        public product products { get; set; }
        public ProductImageModel()
        {
            image imagemodel = new image();
            product propductmodel = new product();

        }
    }
}