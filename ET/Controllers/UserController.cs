using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ET.Models;
using ET.Models.ViewModel;
using ET.Security;

namespace ET.Controllers
{
    public class UserController : Controller
    {
        MembershipEntities db = new MembershipEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        [NonAction]
        public bool isExistUser(string username)
        {
            var result = db.user.Where(a => a.Email == username).FirstOrDefault();
            return result == null ? false:true ;
        }

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;

            //db.Configuration.ValidateOnSaveEnabled = false;
            var result = db.user.Where(a => a.activationCode == new Guid(id).ToString()).FirstOrDefault();
            if (result != null)
            {
                result.isMailVerified = true;
                db.SaveChanges();
                Status = true;
            }
            else
            {
                ViewBag.Message = "Geçersiz istek";
            }

            ViewBag.Status = Status;
            return View();
        }

       
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(user _user)
        {
            string message="";
            bool status=false;
            if (isExistUser(_user.Email))
            {
                ModelState.AddModelError("error", "Bu epostaya ait bir kullanıcı mevcut");
                return View();
            }
            //aktivasyon kodu üretiyoruz.
            _user.activationCode = Guid.NewGuid().ToString();
              //password şifreleniyor
            _user.password = Crypto.Hash(_user.password);
            _user.rePassword = Crypto.Hash(_user.rePassword);

            _user.isMailVerified = false;
            _user.createdDate = DateTime.Now;
            _user.roleId = 2;
            db.user.Add(_user);
            db.SaveChanges();
            // Mail gönder
            SendVerificationLinkEmail(_user.Email, _user.activationCode);
            message = "Kaydınız başarılı şekilde gerçekleştirildi. Hesap aktivasyon linki "
                 + _user.Email + " adresinize gönderilmiştir.Doğrulamayı unutmayınız.";
            status = true;
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View(_user);

        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode)
        {
            SmtpSection network = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            try
            {
                var verifyUrl = "/User/VerifyAccount/" + activationCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var fromEmail = new MailAddress(network.Network.UserName, "Ecommerce Üyeliği");
                var toEmail = new MailAddress(emailID);

                string subject = "Ecommerce Hoşgeldiniz!";
                string body = "<br/><br/>Ecommerce hesabnız başarıyla oluşturulmuştur. Hesabınız aktive etmek için aşağıdaki linke tıklayınız" +
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
        int sayac = 0;
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Signout();
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(Login _login, string ReturnUrl)
        {
            string message = "";
          
            var v = db.user.Where(a => a.Email == _login.Email).FirstOrDefault();
            sayac = v.loginAttempt??0;
            if (v != null)
            {
                bool mailverify = v.isMailVerified ?? false;
                if (!mailverify)
                {
                    ViewBag.Message = "Aktivasyon için mail hesabınızı kontrol edin.";
                    sayac++;
                    v.loginAttempt = sayac;
                    db.SaveChanges();
                    return View();
                }
             

                _login.Password = Crypto.Hash(_login.Password);
                if (string.Compare(_login.Password, v.password) == 0)
                {
                    v.loginTime = DateTime.Now;
                    v.loginAttempt = sayac;
                    db.SaveChanges();
                    Session["username"] = _login.Email;
                    // cookie kalıcılığı için süre
                    int timeout = _login.rememberMe ? 60 : 10;
                    var ticket = new FormsAuthenticationTicket(_login.Email, _login.rememberMe, timeout);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;

                    FormsAuthentication.SetAuthCookie("userName", _login.rememberMe);
                    Response.Cookies.Add(cookie);


                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Default");
                    }
                }
                else
                {
                    sayac++;
                    v.loginAttempt = sayac;
                    db.SaveChanges();
                    message = "Giriş yapılamadı.şifre yanlış";
                }
            }
            else
            {
                sayac++;
                
                db.SaveChanges();
                message = "Giriş yapılamadı.Böyle bir hesap bulunamadı.Lütfen Üye olun.";
            }

            ViewBag.Message = message;
            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            bool Status = false;
            string message = "";
            if (!string.IsNullOrEmpty(email))
            {
                // reset code üret
                Guid resetCode = Guid.NewGuid();
                //girilen Emil adresi sorgulanır
                var v = db.user.Where(o => o.Email == email).FirstOrDefault();
                //email var ise  db'ye reset code yaz
                if (v != null)
                {
                    v.resetCode = resetCode.ToString();
                    db.SaveChanges();
                    // mail gönder
                    SendResetPassword(email, resetCode.ToString());
                    message = "Parola Sıfırlama işlemi başarılı şekilde gerçekleştirildi. Parola sifirlama linki "
                            + email + " adresinize gönderilmiştir.";
                    Status = true;
                }
                else
                {
                    message = "Böyle bir Email mevcut değil";
                    Status = false;
                }

            }
            else
                message = "Email girmeniz gerekiyor";
            ViewBag.message = message;
            ViewBag.status = Status;
            return View();
        }
        [NonAction]
        public void SendResetPassword(string emailID, string resetCode)
        {
            SmtpSection network = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            try
            {
                var verifyUrl = "/User/ResetPassword/" + resetCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
                var fromEmail = new MailAddress(network.Network.UserName, "Ecommerce Parola Sıfırlama");
                var toEmail = new MailAddress(emailID);

                string subject = "Ecommerce Parola sıfırlanacaktır!";
                string body = "<br/><br/>Ecommerce üyeliğinizin parolası sıfırlanacaktır. Parolanızı sıfırlamak için aşağıdaki linke tıklayınız" +
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

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            resetPassword rp = new resetPassword();
            //  set reset code
            rp.resetCode = id;
            return View(rp);
        }
        // reset password
        [HttpPost]
        public ActionResult resetPassword(resetPassword rp)
        {
            bool Status = false;
            string message = "";
            if (ModelState.IsValid)
            {
                var result = db.user.Where(u => u.resetCode == new Guid(rp.resetCode).ToString()).FirstOrDefault();
                result.password = Crypto.Hash(rp.newPassword);
                result.rePassword = Crypto.Hash(rp.comfirmPassword);
                db.SaveChanges();
                Status = true;
                message = "Your password changed succesfully.";

                ViewBag.status = Status;
                ViewBag.message = message;
            }
            return View();
        }
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }


    }
}