using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Globalization;
using System.Web.Mvc;
using telstarapp.Models;
using System.Linq;
using System.Web.UI.WebControls.WebParts;
using System.Web.WebPages;
using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using telstarapp.Services;

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
                    City startCity = db.Cities.Where(city => city.Name.StartsWith(toCity)).FirstOrDefault();
                    City endCity = db.Cities.Where(city => city.Name.StartsWith(fromCity)).FirstOrDefault();

                    List<City> cities = db.Cities.ToList();
                    List<Connection> connections = db.Connections.ToList();
                    //todo use listsForRouting
                    CalculatorService cs = new CalculatorService();
                    //Route, Price, Hours
                    Graph<int, string> cheapGraph = cs.createAndConnectNodes(cities, connections, "Cheapest");
                    Tuple<string, double, int> cheapestTuple = cs.getCheapPath(cheapGraph, startCity, endCity);
                    Graph<int, string> fastestGraph = cs.createAndConnectNodes(cities, connections, "Fastest");
                    Tuple<string, double, int> fastestTuble = cs.getFastPath(fastestGraph, startCity, endCity);


                    Package package = new Package();
                    package.Recommeded = form["Recommended"] == null || form["Recommended"].IsEmpty() ? (byte)0 : (byte)1;
                    package.WeightInKg = form["Weight"] == null || form["Weight"].Equals("") ? 0 : Convert.ToDouble(((String) form["Weight"]));
                    package.HeightInCm = form["Height"] == null || form["Height"].Equals("") ? 0 : Convert.ToInt32(((String) form["Height"]));
                    package.WidthInCm = form["Width"] == null || form["Width"].Equals("") ? 0 : Convert.ToInt32((String)form["Width"]);
                    package.DepthInCm = form["Depth"] == null || form["Depth"].Equals("") ? 0 : Convert.ToInt32((String)form["Depth"]);

                    String specialGood = form["specialGood"];
                    if (specialGood != null)
                    { 
                        if (specialGood.Equals("animals"))
                        {
                            package.SpecialGoods = 7;
                        } else if (specialGood.Equals("Cautios"))
                        {
                            package.SpecialGoods = 1;
                        } else if (specialGood.Equals("refrigerated"))
                        {
                            package.SpecialGoods = 5;
                        }
                    }


                    //Route, Price, Hours
                    //Cheapest of Route, Order, Package
                    Route cheapestRoute = new Route();
                    cheapestRoute.Cities = cheapestTuple.Item1;
                    Order cheapestOrder = new Order();
                    cheapestOrder.OrderId = generateId();
                    cheapestOrder.OurPrice = cheapestTuple.Item2;
                    cheapestOrder.OtherPrice = 0;
                    cheapestOrder.Hours =cheapestTuple.Item3;
                    cheapestOrder.PaidStatus = 0;
                    cheapestOrder.User = Convert.ToInt32(Session["UserID"].ToString());
                    cheapestOrder.Route = cheapestTuple.Item1;

                    //Route, Price, Hours
                    //Fastest of Route, Order, Package
                    Route fastestRoute = new Route();
                    fastestRoute.Cities = fastestTuble.Item1;
                    Order fastestOrder = new Order();
                    fastestOrder.OrderId = generateId();
                    fastestOrder.OurPrice = fastestTuble.Item2;
                    fastestOrder.OtherPrice = 0;
                    fastestOrder.Hours = cheapestTuple.Item3;
                    fastestOrder.PaidStatus = 0;
                    fastestOrder.User = Convert.ToInt32(Session["UserID"].ToString());
                    fastestOrder.Route = cheapestTuple.Item1;

                    //Set session to keep values
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
            int returnValue = ((int) DateTime.Now.ToFileTime());
            return Math.Abs(returnValue);
        }

    }

}