using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ET.Models;
using ET.Models.ViewModel.Account;
using ET.Models.ViewModel;

namespace ET.Controllers
{
    [Authorize(Roles ="User")]
    public class AccountController : Controller
    {
        // GET: Account
        // deneme
        // deneme
        MembershipEntities db = new MembershipEntities();

        public ActionResult Index()
        {
            string userName;
          
            if (User.Identity.IsAuthenticated)
            {
                // userName = Session["username"].ToString();
                userName = User.Identity.Name;
                var result = from u in db.user
                             join ua in db.userToaddress on
                             u.userId equals ua.userId
                             join a in db.address on
                             ua.addressId equals a.addressId
                             where u.Email == userName
                             select new UserAddress
                             {
                                 address = a,
                                 user=u,
                                 Id= ua.Id

                             };
            return View(result);
            }
            return View();
        }
        public ActionResult Create()
        {
            user user = new user();

            if (User.Identity.IsAuthenticated)
            {
                string userName = User.Identity.Name;
                // string username = Session["username"].ToString();
                var availableUser = db.user.Where(x => x.Email == userName).FirstOrDefault();

                UserAddress model = new UserAddress()
                {
                    user = availableUser
                };

                ViewBag.Cities = db.city;
                return View(model);
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(UserAddress ua)
        {
            // mevcut kullanıcının eksik bilgilerini tamamla
            if (User.Identity.IsAuthenticated)
            {
                string userName = User.Identity.Name;
                //string username = Session["username"].ToString();
                var availableUser = db.user.Where(x => x.Email == userName).FirstOrDefault();
                availableUser.lastName = ua.user.lastName ?? "";
                availableUser.firstName = ua.user.firstName ?? "";
                availableUser.phone = ua.user.phone ?? "";
                db.SaveChanges();
                //-------
                //userToaddress  tablosu için data
                user _user = availableUser;
                address _address = ua.address;
                //address tablosuna kaydet
                db.address.Add(_address);
                db.SaveChanges();
                int addressId = _address.addressId;


                userToaddress model = new userToaddress()
                {
                    user = _user,
                    address = _address
                };
                // usertoaddress tablosuna ekle
                db.userToaddress.Add(model);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int Id)
        {
            var model = (from a in db.address
                         join ua in db.userToaddress
                         on a.addressId equals ua.addressId
                         join u in db.user on ua.userId equals u.userId
                         where ua.Id == Id
                         select new UserAddress
                         {
                             address = a,
                             user = u,
                             Id = Id
                         }).SingleOrDefault();
            ViewBag.Cities = db.city;
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserAddress ua)
        {
            if (User.Identity.IsAuthenticated)
            {
                // mevcut kullanıcının eksik bilgilerini tamamla
                string userName = User.Identity.Name;
                var availableUser = db.user.Where(x => x.Email == userName).FirstOrDefault();
                availableUser.lastName = ua.user.lastName ?? "";
                availableUser.firstName = ua.user.firstName ?? "";
                availableUser.phone = ua.user.phone ?? "";
                db.SaveChanges();
                // - address tablosu update ediliyor
                address _address = ua.address;
                db.Entry(_address).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }

        public ActionResult Delete(int Id)
        {
            var model = (from a in db.address
                         join ua in db.userToaddress
                         on a.addressId equals ua.addressId
                         join u in db.user on ua.userId equals u.userId
                         where ua.Id == Id
                         select new UserAddress
                         {
                             address = a,
                             user = u,
                             Id = Id
                         }).SingleOrDefault();
            return View(model);
        }
        [HttpPost,ActionName("Delete")]
        public ActionResult DeleteAddress(int Id)
        {
           

                var model = (from a in db.address
                             join ua in db.userToaddress
                             on a.addressId equals ua.addressId
                             join u in db.user on ua.userId equals u.userId
                             where ua.Id == Id
                             select new UserAddress
                             {
                                 address = a,
                                 user = u,
                                 Id = Id
                             }).SingleOrDefault();

            userToaddress m = db.userToaddress.Where(z=>z.Id==Id).SingleOrDefault();


                // userToaddress tablosundan ilgili kayıt siliniyor
                
                db.userToaddress.Remove(m);
                db.SaveChanges();
                // addres tablosundan ilgili address siliniyor
                address deleteAddress = model.address;
                db.address.Remove(deleteAddress);
                db.SaveChanges();
           
            return RedirectToAction("Index");
        }
        public string deleteWishList(int Id)
        {
            wishList deletedWish = db.wishList.Where(x => x.id == Id).SingleOrDefault();
            db.wishList.Remove(deletedWish);
            db.SaveChanges();
            return "silindi";
        }

        public ActionResult MyWishList()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userName = User.Identity.Name;
                var model = (from u in db.user
                             join w in db.wishList on u.userId equals w.userId
                             join p in db.product on w.productId equals p.productId
                             where u.Email == userName
                             select new MyWishList
                             {
                                 Id =w.id,
                                 product = p,
                                 user = u
                             }).ToList();
                return View(model);
            }
            return RedirectToAction("Login", "User");
        }
        public string deleteMyOrder(int Id)
        {
            orderDetails deletedorder = db.orderDetails.Where(x => x.id == Id).SingleOrDefault();
            db.orderDetails.Remove(deletedorder);
            db.SaveChanges();
           /* orders deleteorder = db.orders.Where(x=>x.orderId==deletedorder.orderId).SingleOrDefault();
            db.orders.Remove(deleteorder);
            db.SaveChanges();*/
            return "silindi";
        }
        public ActionResult MyOrders()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userName = User.Identity.Name;
                var availableUser = db.user.Where(x => x.Email == userName).FirstOrDefault();
             

                var model = from u in db.user join o in db.orders on u.userId equals o.customerId join od in db.orderDetails on o.orderId equals od.orderId join p in db.product on od.productId equals p.productId where u.userId ==availableUser.userId select new orderDetail
                {

                    Id = od.id,
                    product = p,
                    quantity = od.quantity ?? 1,
                    orderId = od.orderId ?? 1,


                };

                return View(model);

            }

            return RedirectToAction("Login", "User");
           
        }

        public JsonResult getDistricts(int cityId)
        {
            db.Configuration.ProxyCreationEnabled = false;

            return Json(db.district.Where(x => x.cityId == cityId), JsonRequestBehavior.AllowGet);
        }
    }
}