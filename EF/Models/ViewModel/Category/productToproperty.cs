using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EF.Models.ViewModel.Category
{
    public class productToproperty
    {
        public category category { get; set; }
        public product product { get; set; }
        public properties property { get; set; }

        public List<propertyValues> propertyValues { get; set; }
        public List<properties> properties { get; set; }
       

        public List<category> mainCategories { get; set; }
        public List<category> subCategories { get; set; }
    }
}