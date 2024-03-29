﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF.Models;
using System.Data.Entity;
using EF.Models.ViewModel.Category;

namespace EF.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category

        MembershipEntities db = new MembershipEntities();
     
        public ActionResult Index()
        {
            //listeleme
            CatSubCat cat = new CatSubCat();

            var model = from c in db.category   join  s in db.category on c.categoryId equals s.parentId into joinedTable from t in joinedTable.DefaultIfEmpty()  select new CatSubCat
            {
                categoryId = c.categoryId,
                categoryName = c.categoryName,
                subcategoryName = t.categoryName??string.Empty,
                desc = c.categoryDesc

            };
           
            return View(model);
        }
       // product/Create ajax
        public ActionResult getsubCategories(int Id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var result = db.category.Where(x => x.parentId == Id).ToList();
            if (result != null)
                return Json(result, JsonRequestBehavior.AllowGet);
            else
                return Content("0");
        }
        public ActionResult Create()
        {
            var t = db.category.Where(x => x.parentId == 0).ToList();
            List<SelectListItem> liste = new List<SelectListItem>();
            liste.Add(new SelectListItem() { Value = "0", Text = "Seçiniz", Selected = true });
            foreach (var item in t)
            {
                liste.Add(new SelectListItem()
                {
                    Text =item.categoryName,
                    Value =item.categoryId.ToString()
                });
            }
            CatSubCat model = new CatSubCat();
            model.categories = liste;
//            ViewBag.categories =new SelectList(liste,"categoryId","categoryName");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(category _category)
        {
            if (ModelState.IsValid)
            {
                // kaydetme
                db.category.Add(_category);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            // seçme
            // select * from  category where categoryId=id
            category model = db.category.Where(a => a.categoryId == id).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(category _category)
        {
            if(ModelState.IsValid)
            {
                db.Entry(_category).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        public ActionResult Delete(int Id)
        {
            category model = db.category.Where(a => a.categoryId == Id).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,int? demo)
        {
            if (ModelState.IsValid)
            {
                category model = db.category.Where(a => a.categoryId == id).FirstOrDefault();
                db.category.Remove(model);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        public ActionResult editProperties(int id)
        {
            properties model = db.properties.Where(p=>p.propertyId==id).FirstOrDefault();
            return View(model);  
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult editProperties(properties model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("properties",new {id=model.categoryId});

        }
        public ActionResult deleteProperties(int id)
        {
            properties model = db.properties.Where(p => p.propertyId == id).FirstOrDefault();
            return View(model);
        }
        
        [HttpPost,ActionName("deleteProperties")]
        [ValidateAntiForgeryToken]
        public ActionResult deleteProperty(int id)
        {
            properties model = db.properties.Where(p => p.propertyId == id).FirstOrDefault();
            db.properties.Remove(model);
            db.SaveChanges();
            return RedirectToAction("properties", new { id = model.categoryId });
        }
        public ActionResult getProperties(int Id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var result = db.properties.Where(x => x.categoryId == Id).ToList();
            if (result != null)
                return Json(result, JsonRequestBehavior.AllowGet);
            else
                return Content("0");
        }
        public ActionResult addProperties(int id)
        {
            productToproperty model = new productToproperty();
           // model.mainCategories = db.category.Where(x => x.parentId == 0).ToList();
            model.category = db.category.FirstOrDefault(x=>x.categoryId==id);
            return View(model);
        }

        public ActionResult GetproductsByCategoryId(int Id)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var result = db.product.Where(x => x.categoryId == Id).ToList();
            if (result != null)
                return Json(result, JsonRequestBehavior.AllowGet);
            else
                return Content("0");
        }
        public ActionResult properties(int? id)
        {
            ViewBag.id = id;
            return View(db.properties.Where(p => p.categoryId == id).ToList());
        }

        [HttpPost]
        public ActionResult addProperties(productToproperty model)
        {
            productToproperty listModel = new productToproperty();

            if (ModelState.IsValid)
            {

                properties prop = new properties()
                {
                    categoryId =  model.category.categoryId,
                    propertyName = model.property.propertyName,
                    propertyType = model.property.propertyType
                };
                db.properties.Add(prop);
                db.SaveChanges();

                // List<properties> listOfproperty = db.properties.ToList();

                listModel.mainCategories = db.category.Where(x => x.parentId == 0).ToList();
                listModel.properties = db.properties.ToList();
            }
            return RedirectToAction("properties",new { id= model.category.categoryId });
                
        }



    }
}