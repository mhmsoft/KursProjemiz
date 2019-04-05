using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EF.Models.ViewModel.Category
{
    public class CatSubCat
    {
        public int categoryId { get; set; }
        public int parentId { get; set; }
        public string categoryName { get; set; }
        public string subcategoryName { get; set; }
        public string desc { get; set; }
        public List<SelectListItem> categories { get; set; }
    }
}