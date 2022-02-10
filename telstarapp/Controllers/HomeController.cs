using System;
using System.Collections.Generic;
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
                return true;
            }
        }

        public bool isAdmin()
        {
            if (Session["isAdmin"].Equals(true))
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
                        var adminUser = db.Users.Where(a => a.Email.Equals(user.Email) && a.Password.Equals(user.Password) && a.Admin != null).FirstOrDefault();
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


        public ActionResult History()
        {
            if (isLoggedIn())
            {
                using (MyEntities db = new MyEntities())
                {
                    List<Order> orders = db.Orders.ToList();
                    ViewBag.foundOrders = orders;

                    return View("History");
                }
            }
            else
            {
                return RedirectToAction("mainPage");
                
            }
        }

        public ActionResult checkRoute()
        {
            using (MyEntities db = new MyEntities())
            {

                City startCity = db.Cities.Where(city => city.Name.Equals("<EnterStartCity")).FirstOrDefault();
                City endCity = db.Cities.Where(city => city.Name.Equals("<EnterEndCity>")).FirstOrDefault();

                List<City> cities = db.Cities.ToList();
                List<Connection> connections = db.Connections.ToList();
                //todo use listsForRouting
                int price = calculateDistance(cities, connections);
                int hours = calculatePrice(cities, connections);


                return View("homePage");
            }
        }

        private int calculateDistance(List<City> cities, List<Connection> connection)
        {
            return 0;
        }

        private int calculatePrice(List<City> cities, List<Connection> connection)
        {
            return 0;
        }
    }


}