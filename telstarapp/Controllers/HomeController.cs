using System;
using System.Web.Mvc;
using telstarapp.Models;
using System.Linq;

namespace telstarapp.Controllers
{
    public class HomeController : Controller
    {
        public bool isLoggedIn()
        {
            if (Session["UserID"] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult loginsubmit(User user)
        {

            if (ModelState.IsValid)
            {
                using (MyEntities db = new MyEntities())
                {
                    var aUser = db.Users.Where(a => a.Email.Equals(user.Email) && a.Password.Equals(user.Password)).FirstOrDefault();
                    
                    if (aUser != null)
                    {
                        Session["UserID"] = aUser.UserId.ToString();
                        var adminUser = db.Users.Where(a => a.Email.Equals(user.Email) && a.Password.Equals(user.Password)).FirstOrDefault();
                        if (adminUser != null)
                        {
                            Session["isAdmin"] = true;
                        }
                        return RedirectToAction("mainPage");
                    }
                }

            }
            return RedirectToAction("Index");
        }

        public ActionResult mainPage()
        {
            if (isLoggedIn())
            {
                return View("Main");
            }
            else
            {
                {
                    return RedirectToAction("Index");
                }
            }
        }

        public ActionResult logOut()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}