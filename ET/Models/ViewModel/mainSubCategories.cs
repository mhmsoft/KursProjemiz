using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ET.Models.ViewModel
{
    public class mainSubCategories
    {
        
            public int categoryId { get; set; }
            public int parentId { get; set; }
            public string categoryName { get; set; }
            public string subcategoryName { get; set; }
            public string desc { get; set; }
            public List<SelectListItem> categories { get; set; }
            public List<category> subCategories { get; set; }
            public List<category> mainCategories { get; set; }
            public List<category> allCategories { get; set; }

    }
}