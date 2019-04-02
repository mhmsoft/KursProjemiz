using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ET.Models;
using ET.Models.ViewModel;
using PagedList;


namespace ET.Controllers
{
    public class DefaultController : Controller
    {
        MembershipEntities db = new MembershipEntities();
        // GET: Default
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult categories()
        {
            return PartialView(db.category.ToList());
        }
        public PartialViewResult brands()
        {

            var query =from p in db.product join b in db.brand
                       on p.brandId equals b.brandId
                       group new { b, p } by new { b.brandName,b.brandId } into g
                       select new Product2Brand
                       {
                           brandId = g.Key.brandId,
                           brandName = g.Key.brandName,
                           Count = g.Select(m =>         m.p.productId).Distinct().Count()
                       };

            return PartialView(query);
        }
        public PartialViewResult Search()
        {
            return PartialView();
        }

        public ActionResult Products(int?categoryId,int?brandid,string searchValue,int?page)
        {
            int pageSize = 3;
            // page boş değilse kendisi değilse default 1 olsun
            int pageNumber = (page ?? 1);

            if (searchValue!=null)
            {
                var query = from p in db.product
                            join i in db.images
                            on p.productId equals i.productId
                            where (i.isShow == true)
                            select new product2Image
                            {
                                image = i,
                                products = p

                            };
                query = query.Where(x => x.products.productName.Contains(searchValue) || x.products.brand.brandName.Contains(searchValue) || x.products.category.categoryName.Contains(searchValue)).OrderBy(x=>x.products.productName);
                return View(query.ToPagedList(pageNumber, pageSize));

            }
            if(categoryId!=null)
            {
               var query = from p in db.product
                            join i in db.images
       on p.productId equals i.productId
                            where (i.isShow == true && p.categoryId==categoryId)
                            select new product2Image
                            {
                                image = i,
                                products = p

                            };
                query = query.OrderBy(s => s.products.productName);
                return View(query.ToPagedList(pageNumber, pageSize));
            }
            // Marka seçildiğinde
            if (brandid != null)
            {
                var m = (from p in db.product
                         join i in db.images on p.productId equals i.productId
                         where (i.isShow == true && p.brandId == brandid)
                         select new product2Image { products = p, image = i });
                m = m.OrderBy(s => s.products.productName);
                return View(m.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var query = from p in db.product
                            join i in db.images
       on p.productId equals i.productId
                            where (i.isShow == true)
                            select new product2Image
                            {
                                image = i,
                                products = p

                            };
                query = query.OrderBy(s => s.products.productName);
                return View(query.ToPagedList(pageNumber, pageSize));
            }

            
        }
        [Authorize]
        public ActionResult ProductDetails(int productId)
        {

            string t = db.Database.SqlQuery<string>("Select brandName From brand b , product p  Where b.brandId=p.brandId and productId=" + productId).FirstOrDefault();
            
            product2Image PIm = new product2Image()
            {
                imageList = db.images.Where(x => x.productId == productId).ToList(),
                products = db.product.Where(x => x.productId == productId).FirstOrDefault(),
                brand = t
            };
            return View(PIm);
        }

        public bool isExists(int productId)
        {
            var result = db.wishlist.FirstOrDefault(x => x.productId == productId);
            return result != null;
        }
        
        public ActionResult addWishList(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {

                return RedirectToAction("Login","User");
            }

            string username = User.Identity.Name;
            var availableUser = db.user.Where(x => x.Email == username).FirstOrDefault();

            if (!isExists(productId))
            {
                product y = db.product.Where(x => x.productId == productId).FirstOrDefault();
                wishlist model = new wishlist()
                {
                    product = y,
                    user = availableUser
                };
                db.wishlist.Add(model);
                db.SaveChanges();
                return  Content("Eklendi.");
            }
            return Content("Eklenemedi.");


        }

        [NonAction]
        private int isExistInCard(int id)
        {
            List<BasketItem> card = (List<BasketItem>)Session["card"];
            for (int i = 0; i < card.Count; i++)
                if (card[i].product.productId.Equals(id))
                    return i;
            return -1;
        }
        public string addcart(int productId, int quantity)
        {
            return AddCard(productId, quantity);
        }
        public string AddCard(int productId, int quantity)
        {
            string message = "";
            product _product = db.product.FirstOrDefault(x => x.productId == productId);
            images _image = db.images.FirstOrDefault(x => x.productId == productId && x.isShow == true);
          
            if (Session["card"] == null)
            {
                List<BasketItem> Card = new List<BasketItem>();
                Card.Add(new BasketItem()
                {
                    product = _product,
                    quantity = quantity,
                    img = _image,
                    DateCreated = DateTime.Now
                });
                Session["card"] = Card;
                message = "Eklendi";

            }
            else
            {
                List<BasketItem> card = (List<BasketItem>)Session["card"];
                // spette eklenen ürünün  sepetteki sıra numarasına bakılır. varsa sepetteki sıra no gönderilir, yoksa -1 değeri gönderilir.
                int index = isExistInCard(productId);
                // sepette eklenen ürün varsa
                if (index != -1)
                {
                    // sadece adedini gelen quantity kadar arttıracak.
                    card[index].quantity+=quantity;
                }
                // sepette girilen ürün yoksa 
                else
                {
                    // sepete ekle
                    card.Add(new BasketItem
                    {
                        product = _product,
                        quantity = quantity,
                        img = _image,
                        DateCreated = DateTime.Now
                    });
                }
                Session["card"] = card;
                message = "karta eklendi";
            }
            return message;
        }
        // sepetteki elemanı silme
        public string Remove(int productId)
        {
            string message = "";
            List<BasketItem> card = (List<BasketItem>)Session["card"];
            if (card.Exists(x => x.product.productId == productId))
            {
                int index = isExistInCard(productId);

                card.RemoveAt(index);
                Session["card"] = card;
                message = "Silindi";
            }
            return message;
        }
        public ActionResult Basket()
        {
            return View();
        }


    }
}