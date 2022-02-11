using System;
using System.Collections.Generic;
using System.Web.Mvc;
using telstarapp.Models;
using System.Linq;
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

        public ActionResult checkRoute()
        {
            using (MyEntities db = new MyEntities())
            {

                City startCity = db.Cities.Where(city => city.Name.StartsWith("Addis")).FirstOrDefault();
                City endCity = db.Cities.Where(city => city.Name.StartsWith("Zan")).FirstOrDefault();
                List<City> cities = db.Cities.ToList();
                List<Connection> connections = db.Connections.ToList();
                //todo use listsForRouting
                CalculatorService cs = new CalculatorService();
               Graph<int, string> graph = cs.createAndConnectNodes(cities, connections);
                Console.WriteLine(cs);

                Tuple<string, double, int> test = cs.getShortestRoute(graph, startCity, endCity);



                return RedirectToAction("mainPage");
            }
        }

    }


}