using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF.Models;

namespace EF.Controllers
{
    public class ProductController : Controller
    {
        MembershipEntities db = new MembershipEntities();
        // GET: Product
        public ActionResult Index()
        {
            ProductImageModel model= new ProductImageModel();
            model.Productlist = db.products.ToList();
            return View(model.Productlist);
        }

        public ActionResult Create()
        {
            ViewBag.categories = db.categories.ToList();
            ViewBag.brands = db.brands.ToList();
            return View();
        }
        [HttpPost]
        public ActionResult Create(product _product,IEnumerable<HttpPostedFileBase> img)
        {
            db.products.Add(_product);
            db.SaveChanges();

            image new_img = new image();
            new_img.productId = _product.productId;
            new_img.isShow = false;
            if (img.First() != null)
            {
                foreach (var item in img)
                {
                    using (var br = new BinaryReader(item.InputStream))
                    {
                        var data = br.ReadBytes(item.ContentLength);
                        new_img.imagepath = data;
                        db.images.Add(new_img);
                        db.SaveChanges();
                    }
                }

            }
            ViewBag.categories = db.categories.ToList();
            ViewBag.brands = db.brands.ToList();
            return View();
        }

        public ActionResult Details(int id)
        {
            ProductImageModel pim = new ProductImageModel()
            {
                products = db.products.Where(a => a.productId == id).FirstOrDefault(),
                Imagelist = db.images.Where(a => a.productId == id).ToList()
        };
            return View(pim);
        }
        public ActionResult Edit(int id)
        {
            ProductImageModel pim = new ProductImageModel()
            {
                products = db.products.Where(a => a.productId == id).FirstOrDefault(),
                Imagelist = db.images.Where(a => a.productId == id).ToList()
            };
            //başlangıçta Categori listesi viEw' e gönderiyoruz
            ViewBag.categories = new SelectList(db.categories,"categoryId","categoryName",pim.products.categoryId);
            ViewBag.brands = new SelectList(db.brands, "brandId", "brandName", pim.products.brandId);
            return View(pim);
        }
        [HttpPost]
        public ActionResult Edit(ProductImageModel _productImage, IEnumerable<HttpPostedFileBase> img)
        {
            product _product= _productImage.products;
            db.Entry(_product).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            if (img.First() != null)
            {
                image new_img = new image();
                new_img.isShow = false;
                new_img.productId = _product.productId;
                foreach (var item in img)
                {
                    using (var br = new BinaryReader(item.InputStream))
                    {
                        var data = br.ReadBytes(item.ContentLength);
                        new_img.imagepath = data;
                        db.images.Add(new_img);
                        db.SaveChanges();
                    }
                }

            }

            return RedirectToAction("Index");
            
                       
        }
        public ActionResult Delete(int id)
        {
            return View(db.products.Where(a => a.productId == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(int id,int? c)
        {
            product model= db.products.Where(a => a.productId ==id).FirstOrDefault();
            db.products.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public string DeleteImg(int image_id)
        {
            string islem="";
            image img = db.images.Where(a => a.imageId == image_id).FirstOrDefault();
            if (img!=null)
            { 

            db.images.Remove(img);
            db.SaveChanges();
             System.IO.File.Delete(Server.MapPath("~/Content/uploads/" + img.imagepath.ToString()));
                islem = "Silme işlemi Yapıldı";
            }
            return islem;
            

        }
        public string SetDefaultImage(int image_id,int product_Id)
        {

            List<image> allImages = db.images.Where(x=>x.productId==product_Id).ToList();

            foreach (var item in allImages)
            {
                item.isShow = false;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            image img = db.images.Where(a => a.imageId == image_id).FirstOrDefault();
            img.isShow = true;
            string mesaj="Değişiklik yok";
            if (img != null)
            {
                db.Entry(img).State= System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                mesaj = "değişlikler yapıldı";
            }
            return  mesaj;

        }

    }
}