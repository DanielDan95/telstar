using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.Mvc;
using telstarapp.Models;
using System.Linq;
using System.Web.WebPages;
using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;

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
            ViewBag.isLoggedIn = isLoggedIn();
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
                using (MyEntities db = new MyEntities())
                {
                    List<City> cities = db.Cities.ToList();
                    ViewBag.foundCities = cities;
                }
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

        public bool isValidForm()
        {
            return true;
        }

        [HttpPost]
        public ActionResult checkRoute(FormCollection form)
        {
            if (isValidForm())
            {
                using (MyEntities db = new MyEntities())
                {
                    String toCity = form["toCity"];
                    String fromCity = form["fromCity"];
                    City startCity = db.Cities.Where(city => city.Name.Equals(toCity)).FirstOrDefault();
                    City endCity = db.Cities.Where(city => city.Name.Equals(fromCity)).FirstOrDefault();

                    List<City> cities = db.Cities.ToList();
                    List<Connection> connections = db.Connections.ToList();
                    //todo use listsForRouting

                    //Cheapest of Route, Order, Package
                    Route cheapestRoute = new Route();
                    //todo after fix algorithm
                    cheapestRoute.Cities = "cheapestRoute";

                    Package package = new Package();
                    package.Recommeded = form["Recommended"] == null || form["Recommended"].IsEmpty() ? (byte)0 : (byte)1;
                    package.WeightInKg = form["Weight"] == null || form["Weight"].IsEmpty() ? 0 : Convert.ToDouble(form["Weight"]);
                    package.HeightInCm = form["Height"] == null || form["Height"].IsEmpty() ? 0 : Convert.ToInt32(form["Height"]);
                    package.HeightInCm = form["Width"] == null || form["Width"].IsEmpty() ? 0 : Convert.ToInt32(form["Width"]);
                    package.HeightInCm = form["Depth"] == null || form["Depth"].IsEmpty() ? 0 : Convert.ToInt32(form["Depth"]);


                    Order cheapestOrder = new Order();
                    //todo fix after algorithm
                    cheapestOrder.OrderId = generateId();
                    cheapestOrder.OurPrice = 55;
                    cheapestOrder.OtherPrice = 66;
                    cheapestOrder.Hours = 4;
                    cheapestOrder.PaidStatus = 0;
                    cheapestOrder.User = 888888888;
                    cheapestOrder.Route = "cheapestRoute";


                    //Fastest of Route, Order, Package
                    Route fastestRoute = new Route();
                    //todo after fix algorithm
                    fastestRoute.Cities = "fastestRoute";



                    Order fastestOrder = new Order();
                    //todo fix after algorithm
                    fastestOrder.OrderId = generateId();
                    fastestOrder.OurPrice = 55;
                    fastestOrder.OtherPrice = 66;
                    fastestOrder.Hours = 4;
                    fastestOrder.PaidStatus = 0;
                    fastestOrder.User = 888888888;
                    fastestOrder.Route = "fastestRoute";
                    Session["hasARoute"] = true;
                    Session["package"] = package;
                    Session["fastestOrder"] = fastestOrder;
                    Session["fastestRoute"] = fastestRoute;
                    Session["cheapestOrder"] = cheapestOrder;
                    Session["cheapestRoute"] = cheapestRoute;

                }
            }
            return RedirectToAction("mainPage");
        }


        public int generateId()
        {
            return (int)DateTime.Now.ToFileTime();
        }

    }

}