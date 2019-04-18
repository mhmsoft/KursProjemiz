using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF.Models;
using System.Data.Entity;
using EF.Models.ViewModel.Account;

namespace EF.Controllers
{
    public class MemberController : Controller
    {
        // GET: Member
        MembershipEntities db = new MembershipEntities();
        public ActionResult Index()
        {
            var result = from u in db.user
                         join r in db.Role on u.roleId equals r.roleId
                         select new UserAddress
                         {
                             role= r,
                             user = u 
                         };
            return View(result);
        }
        public ActionResult Details(int id)
        {
            var result = from u in db.user
                         join ua in db.userToaddress on
                         u.userId equals ua.userId
                         join a in db.address on
                         ua.addressId equals a.addressId
                         join r in db.Role on u.roleId equals r.roleId

                         select new UserAddress
                         {
                             role = r,
                             address = a,
                             user = u,
                             Id = ua.Id

                         };
            return View(result);
        }
        public ActionResult Edit(int id)
        {
            user model = db.user.FirstOrDefault(x=>x.userId==id);
            ViewBag.roles = new SelectList(db.Role.ToList(),"roleId","roleName",model.roleId);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(user user)
        {
            if(ModelState.IsValid)
            {
                user model = db.user.FirstOrDefault(x=>x.userId==user.userId);
                model.isMailVerified = user.isMailVerified;
                model.isActive = user.isActive;
                model.roleId = user.roleId;
               // db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}