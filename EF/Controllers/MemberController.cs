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
        public ActionResult Details(int userId)
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
    }
}