using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF.Models;

namespace EF.Controllers
{
    public class BrandController : Controller
    {
        MembershipEntities db_ = new MembershipEntities(); 
        // GET: Brand
        public ActionResult Index()
        {
            return View(db_.brand.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(brand _brand)
        {   
            if(ModelState.IsValid)
            {
                db_.brand.Add(_brand);
                db_.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("","Model Hatası");
                return View();
            }
               
            
        }
        public ActionResult Edit(int Id)
        {
            brand brand_model = db_.brand.FirstOrDefault(x=>x.brandId==Id);
            return View(brand_model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(brand _brand)
        {
            if (ModelState.IsValid)
            {
                db_.Entry(_brand).State = System.Data.Entity.EntityState.Modified;
                db_.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Model Hatası");
                return View();
            }
           
        }
        public ActionResult Delete(int Id)
        {
            brand brand_model = db_.brand.FirstOrDefault(x => x.brandId == Id);
            return View(brand_model);
        }
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteBrand(int Id)
        {
            brand brand_model = db_.brand.FirstOrDefault(x => x.brandId == Id);
            if (brand_model != null)
            {
                db_.brand.Remove(brand_model);
                db_.SaveChanges();
                return RedirectToAction("index");
            }
            else

            return View();
           
        }


    }
}