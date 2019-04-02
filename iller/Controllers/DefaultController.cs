using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iller.Models;

namespace iller.Controllers
{
    public class DefaultController : Controller
    {
        private readonly ilEntity Db = new ilEntity();

        // GET: Default
        public ActionResult Index()
        {
            return View(Db.Iller.ToList());
        }
        [HttpPost]
        public ActionResult ilceGetir(int id)
        {
            Db.Configuration.ProxyCreationEnabled = false;
            var result = Db.Ilceler.Where(a => a.IlID == id).ToList();
            return Json(result);
        }
    }
}