using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF.Models;
using EF.Models.ViewModel.Category;
namespace EF.Controllers
{
    public class ProductController : Controller
    {
        MembershipEntities db = new MembershipEntities();
        // GET: Product
        public ActionResult Index()
        {
            ProductImageModel model= new ProductImageModel();
            model.Productlist = db.product.ToList();
            return View(model.Productlist);
        }

        public ActionResult Create()
        {
            var t = db.category.Where(x =>x.parentId == 0).ToList();
            List<SelectListItem> liste = new List<SelectListItem>();
            liste.Add(new SelectListItem() { Value = "0", Text = "Seçiniz", Selected = true });
            foreach (var item in t)
            {
                liste.Add(new SelectListItem()
                {
                    Text = item.categoryName,
                    Value = item.categoryId.ToString()
                });
            }
            ViewBag.categories = liste;
            ViewBag.brands = db.brand.ToList();
            return View();
        }
        
        
        [HttpPost]
        public ActionResult Create(productToproperty _product,IEnumerable<HttpPostedFileBase> img,int subCategoryId,propertyValues[] propertyValues)
        {
            if (subCategoryId != null)
            {
                _product.product.categoryId = subCategoryId;
            }
            db.product.Add(_product.product);
            db.SaveChanges();
            foreach (var item in _product.propertyValues)
            {
                item.productId = _product.product.productId;
                
                db.propertyValues.Add(item);
                db.SaveChanges();
            }
           

            images new_img = new images();
            new_img.productId = _product.product.productId;
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
            var t = db.category.Where(x => x.parentId == 0).ToList();
            List<SelectListItem> liste = new List<SelectListItem>();
            liste.Add(new SelectListItem() { Value = "0", Text = "Seçiniz", Selected = true });
            foreach (var item in t)
            {
                liste.Add(new SelectListItem()
                {
                    Text = item.categoryName,
                    Value = item.categoryId.ToString()
                });
            }
            ViewBag.categories = liste;
            ViewBag.brands = db.brand.ToList();
            return View();
        }

        public ActionResult Details(int id)
        {
            ProductImageModel pim = new ProductImageModel()
            {
                products = db.product.Where(a => a.productId == id).FirstOrDefault(),
                Imagelist = db.images.Where(a => a.productId == id).ToList()
        };
            return View(pim);
        }
        public ActionResult Edit(int id)
        {
            /* ProductImageModel pim = new ProductImageModel()
             {
                 products = db.product.Where(a => a.productId == id).FirstOrDefault(),
                 Imagelist = db.images.Where(a => a.productId == id).ToList()
             };
             */
            product productModel = db.product.Where(a => a.productId == id).FirstOrDefault();
           
            int maincat = productModel.category.parentId??0;
            if( maincat==0)
            {
                //başlangıçta Categori listesi viEw' e gönderiyoruz
                ViewBag.categories = new SelectList(db.category.Where(x => x.parentId == 0).ToList(), "categoryId", "categoryName", productModel.categoryId);
            }
            else
            {
                ViewBag.categories = new SelectList(db.category.Where(x => x.parentId == 0).ToList(), "categoryId", "categoryName", maincat);
                ViewBag.subcategories = new SelectList(db.category.Where(x => x.parentId == maincat).ToList(), "categoryId", "categoryName", id);
            }
           
            ViewBag.brands = new SelectList(db.brand, "brandId", "brandName", productModel.brandId);
            return View(productModel);
        }
        [HttpPost]
        public ActionResult Edit(product productModel, IEnumerable<HttpPostedFileBase> img, int? subCategoryId, ICollection<propertyValues> propertyValues)
        {

            if (subCategoryId != null)
            {
                productModel.categoryId = subCategoryId;
            }
           // product updates
            db.Entry(productModel).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            // property Value updates
            if (productModel.propertyValues.First()!=null)
            {
                foreach (var item in productModel.propertyValues)
                {
                   
                    db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
           
            if (img.First() != null)
            {
                images new_img = new images();
                new_img.isShow = false;
                new_img.productId = productModel.productId;
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
            return View(db.product.Where(a => a.productId == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(int id,int? c)
        {
            // remove From  wishlist table
            wishlist WishProducts = db.wishlist.Where(a => a.productId == id).FirstOrDefault();
            if(WishProducts!=null)
            db.wishlist.Remove(WishProducts);
            db.SaveChanges();
            // remove From Images table
            List<images> productImages= db.images.Where(a => a.productId == id).ToList();
            foreach (var item in productImages)
            {
                db.images.Remove(item);
                db.SaveChanges();
            }
            // remove from product table
            product model = db.product.Where(a => a.productId == id).FirstOrDefault();
            db.product.Remove(model);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public string DeleteImg(int image_id)
        {
            string islem="";
            images img = db.images.Where(a => a.imageId == image_id).FirstOrDefault();
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
            List<images> allImages = db.images.Where(x=>x.productId==product_Id).ToList();

            foreach (var item in allImages)
            {
                item.isShow = false;
                db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            images  img = db.images.Where(a => a.imageId == image_id).FirstOrDefault();
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