using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EF.Models.ViewModel.Product
{
    public class productToproperty
    {
        public product product { get; set; }
        public List<properties> property { get; set; }
        public List<category> mainCategories { get; set; }
        public List<category> subCategories { get; set; }
    }
}