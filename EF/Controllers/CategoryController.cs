using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF.Models;
using System.Data.Entity;

namespace EF.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category

        MembershipEntities db = new MembershipEntities();
     
        public ActionResult Index()
        {
            //listeleme
            return View(db.categories.ToList());
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(category _category)
        {
            if (ModelState.IsValid)
            {
                // kaydetme
                db.categories.Add(_category);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            // seçme
            // select * from  category where categoryId=id
            category model = db.categories.Where(a => a.categoryId == id).FirstOrDefault();
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
            category model = db.categories.Where(a => a.categoryId == Id).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,int? demo)
        {
            if (ModelState.IsValid)
            {
                category model = db.categories.Where(a => a.categoryId == id).FirstOrDefault();
                db.categories.Remove(model);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
    }
}