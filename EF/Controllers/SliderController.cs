using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EF.Models;

namespace EF.Controllers
{
    public class SliderController : Controller
    {
        MembershipEntities db = new MembershipEntities();
        // GET: Slider
    
        public ActionResult Index()
        {
            return View(db.slider);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(slider slider,HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    using (var br = new BinaryReader(img.InputStream))
                    {
                        var data = br.ReadBytes(img.ContentLength);
                        slider.imagepath = data;
                        db.slider.Add(slider);
                        db.SaveChanges();
                    }
                }
            }
          
            return RedirectToAction("Index");
        }
        public ActionResult Edit(int id)
        {
            return View(db.slider.FirstOrDefault(x=>x.sliderId==id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(slider slider, HttpPostedFileBase img)
        {
            if (ModelState.IsValid)
            {
                if (img != null)
                {
                    using (var br = new BinaryReader(img.InputStream))
                    {
                        var data = br.ReadBytes(img.ContentLength);
                        slider.imagepath = data;
                        db.SaveChanges();

                    }
                    db.Entry(slider).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                db.Entry(slider).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        public ActionResult Delete(int id)
        {
            return View(db.slider.FirstOrDefault(x => x.sliderId == id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id,int?test)
        {
            if(ModelState.IsValid)
            {
                slider slider = db.slider.FirstOrDefault(x => x.sliderId == id);
                db.slider.Remove(slider);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Thumbnail(int width, int height, int Id)
        {

            var photo = db.slider.Find(Id).imagepath;
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
    }
}