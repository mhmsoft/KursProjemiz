using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EF.Models
{
    public class ProductImageModel
    {
        public List<images> Imagelist { get; set; }
        public List<product> Productlist { get; set; }
        public images Image { get; set; }
        public product products { get; set; }
        public ProductImageModel()
        {
            images imagemodel = new images();
            product propductmodel = new product();

        }
    }
}