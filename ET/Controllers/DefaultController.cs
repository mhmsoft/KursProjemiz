using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ET.Models;
using ET.Models.ViewModel;
using PagedList;
using ET.Models.ViewModel.Account;
using System.Net.Configuration;
using System.Configuration;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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

            var query = from p in db.product
                        join b in db.brand
   on p.brandId equals b.brandId
                        group new { b, p } by new { b.brandName, b.brandId } into g
                        select new Product2Brand
                        {
                            brandId = g.Key.brandId,
                            brandName = g.Key.brandName,
                            Count = g.Select(m => m.p.productId).Distinct().Count()
                        };

            return PartialView(query);
        }
        public PartialViewResult Search()
        {
            return PartialView();
        }
        public ActionResult Thumbnail(int width, int height, int Id)
        {
            // TODO: the filename could be passed as argument of course
            var photo = db.images.Find(Id).imagePath;
            var base64 = Convert.ToBase64String(photo);
            // Convert Base64 String to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);

            using (var newImage = new Bitmap(width, height))
            using (var graphics = Graphics.FromImage(newImage))
            using (var stream = new MemoryStream())
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(image, new Rectangle(0, 0, width, height));
                newImage.Save(stream, ImageFormat.Png);
                return File(stream.ToArray(), "image/png");
            }

        }

        public ActionResult Products(int? categoryId, int? brandid, string searchValue, int? page)
        {
            int pageSize = 3;
            // page boş değilse kendisi değilse default 1 olsun
            int pageNumber = (page ?? 1);

            if (searchValue != null)
            {
                var query = from p in db.product
                            join i in db.images
                            on p.productId equals i.productId
                            where (i.isshow == true)
                            select new product2Image
                            {
                                image = i,
                                products = p

                            };
                query = query.Where(x => x.products.productName.Contains(searchValue) || x.products.brand.brandName.Contains(searchValue) || x.products.category.categoryName.Contains(searchValue)).OrderBy(x => x.products.productName);
                return View(query.ToPagedList(pageNumber, pageSize));

            }
            if (categoryId != null)
            {
                var query = from p in db.product
                            join i in db.images
       on p.productId equals i.productId
                            where (i.isshow == true && p.categoryId == categoryId)
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
                         where (i.isshow == true && p.brandId == brandid)
                         select new product2Image { products = p, image = i });
                m = m.OrderBy(s => s.products.productName);
                return View(m.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                var query = from p in db.product
                            join i in db.images
       on p.productId equals i.productId
                            where (i.isshow == true)
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
            var result = db.wishList.FirstOrDefault(x => x.productId == productId);
            return result != null;
        }

        public ActionResult addWishList(int productId)
        {
            if (!User.Identity.IsAuthenticated)
            {

                return RedirectToAction("Login", "User");
            }

            string username = User.Identity.Name;
            var availableUser = db.user.Where(x => x.Email == username).FirstOrDefault();

            if (!isExists(productId))
            {
                product y = db.product.Where(x => x.productId == productId).FirstOrDefault();
                wishList model = new wishList()
                {
                    product = y,
                    user = availableUser
                };
                db.wishList.Add(model);
                db.SaveChanges();
                return Content("Eklendi.");
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
            images _image = db.images.FirstOrDefault(x => x.productId == productId && x.isshow == true);

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
                    card[index].quantity += quantity;
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
        [Authorize(Roles = "User")]
        public ActionResult CheckOut()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            user availableUser = db.user.Where(x => x.Email == User.Identity.Name).FirstOrDefault();

            UserAddress model = new UserAddress()
            {
                user = availableUser,
                addressList = db.userToaddress.Where(x => x.userId == availableUser.userId).ToList()
            };


            return View(model);

        }
        public void setDefaultAddress(int addressId)
        {
            if (User.Identity.IsAuthenticated)
            {
                user availableUser = db.user.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
                availableUser.addressId = addressId;
                db.SaveChanges();
            }


        }
        public ActionResult completeCheckOut()
        {
            bool status = false;
            string message = "";
            if(!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }
            user availableUser = db.user.Where(x => x.Email == User.Identity.Name).FirstOrDefault();
            orders newOrder = new orders()
            {
                orderDate = DateTime.Now,
                customerId = availableUser.userId

            };
            db.orders.Add(newOrder);
            db.SaveChanges();

            if (Session["card"] != null)
            {
                List<BasketItem> Basket = (List<BasketItem>)Session["card"];
                orderDetails newOrderDetail = new orderDetails();
                foreach (var item in Basket)
                {
                    newOrderDetail.orderId = newOrder.orderId;
                    newOrderDetail.productId = item.product.productId;
                    newOrderDetail.quantity = item.quantity;
                   
                    db.orderDetails.Add(newOrderDetail);
                    db.SaveChanges();
                }
                // mail Gönderecek
                SendOrderInfo(availableUser.Email);
                message = " Sipariş işlemi tamamlandı. siparişiniz ile ilgili bilgi mailinize gönderilmiştir. <br/>" +
                  "Ecommerce sayfanızda sipariş detaylarını görebilirisiniz. Detay için aşağıdaki linke tıklayınız" +
                      " <br/><br/><a href='/Account/MyOrders'></a> ";

            }
            return Content(message);

        }
        [NonAction]
        public void SendOrderInfo(string emailID)
        {
            SmtpSection network = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            try
            {
                var url = "/Account/MyOrders";
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, url);
                var fromEmail = new MailAddress(network.Network.UserName, "Ecommerce Sipariş Bilgisi");
                var toEmail = new MailAddress(emailID);

                string subject = "Ecommerce Sipariş Bilgisi";
                string body = "<br/><br/>Ecommerce sayfanızda sipariş detaylarını görebilirisiniz. Detay için aşağıdaki linke tıklayınız" +
                      " <br/><br/><a href='" + link + "'>" + link + "</a> ";
                var smtp = new SmtpClient
                {
                    Host = network.Network.Host,
                    Port = network.Network.Port,
                    EnableSsl = network.Network.EnableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = network.Network.DefaultCredentials,
                    Credentials = new NetworkCredential(network.Network.UserName, network.Network.Password)
                };
                using (var message = new MailMessage(fromEmail, toEmail)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                })
                    smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}