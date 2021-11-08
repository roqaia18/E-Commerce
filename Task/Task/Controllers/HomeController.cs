using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Task.Models;

namespace Task.Controllers
{
    public class HomeController : Controller
    {
        EcommerceEntities Context = new EcommerceEntities();
        public ActionResult Index()
        {
            List<Item> items = new List<Item>();
            items = Context.Items.ToList();
            return View(items);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexAdmin()
        { 
            return View();
        }
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registration(User user, string returnUrl)
        {
            var Role = user.Role;
            if (ModelState.IsValid)
            {
                FormsAuthentication.SetAuthCookie(user.Email, false);
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                         && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    Context.Users.Add(user);
                    Context.SaveChanges();
                    if (Role =="User")
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("IndexAdmin");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email or Password");
                return View();
            }
        }
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User user, string returnUrl)
        {

            var UserItem = Context.Users.Where(x => x.Email == user.Email && x.Password == user.Password).FirstOrDefault();
            if (UserItem != null)
            {
                FormsAuthentication.SetAuthCookie(UserItem.Email, false);
                if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                         && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    if (UserItem.Role =="User")
                        return RedirectToAction("Index");
                    else
                        return RedirectToAction("IndexAdmin");

                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Email or Password");
                return View();
            }
        }
        [Authorize]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Home");
        }
    }
}