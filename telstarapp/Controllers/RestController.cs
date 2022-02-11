using Dijkstra.NET.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using telstarapp.Models;
using telstarapp.Services;
using telstarapp.theSystem.Connector;

namespace telstarapp.Controllers
{
    public class RestController : ApiController
    {

        [Route("api/Rest/findPriceAndTime")]
        [HttpGet]
        // TODO: fix this
        public IHttpActionResult getTimeAndPrice(
           [FromUri] String from = null,
           [FromUri] String to = null,
           [FromUri] Double weight = 0.0,
           [FromUri] Dictionary<string, int> size = null,
           [FromUri] Boolean recommended = false,
           [FromUri] Boolean weapon = false,
           [FromUri] Boolean cautious = false,
           [FromUri] Boolean refrigerated = false,
           [FromUri] Boolean animal = false
           )
        {
                if (!isValidInput(from, to, weight, size, recommended, weapon, cautious, refrigerated, animal))
            {
                return NotFound();
            }
            var model = new RequestModel(size, from, to, weight, recommended, weapon, cautious, refrigerated, animal);
            var responseModel = GetResponseFromCalculator(model);
            return Json(responseModel);
        }

        [Route("api/Rest/getPriceAndTimeFromOceanic")]
        [HttpGet]
        public async Task<IHttpActionResult> getPriceAndTimeFromOceanic()
        {
            var integrationService = new IntegrationService();
            var timeAndPrice = await integrationService.GetTimeAndPriceOceanic();
            return Json(timeAndPrice);
        }

        [Route("api/Rest/getPriceAndTimeFromEastIndia")]
        [HttpGet]
        public async Task<IHttpActionResult> getPriceAndTimeFromEastIndia()
        {
            var integrationService = new IntegrationService();
            var timeAndPrice = await integrationService.GetTimeAndPriceEastIndia();
            return Json(timeAndPrice);
        }

        private bool isValidInput(String from, String to, Double weight, Dictionary<string, int> size, Boolean recommended, Boolean weapon, Boolean cautious, Boolean refrigerated, Boolean animal)
        {
            using (MyEntities db = new MyEntities())
            {
                City startCity = db.Cities.Where(city => city.Name.Replace("\n", "").Replace("\r", "").Equals(from.Replace("\n", "").Replace("\r", ""))).FirstOrDefault();
                City endCity = db.Cities.Where(city => city.Name.Replace("\n", "").Replace("\r", "").Equals(to.Replace("\n", "").Replace("\r", ""))).FirstOrDefault();
                if (startCity == null || endCity == null)
                {
                    return false;
                }
            }
            
            return from != null && to != null  && size != null;
        }

        private Dictionary<String, TimeAndPrice> GetResponse(RequestModel SearchModel)
        {

            double cheapestPrice = SearchModel.from.Length;
            double fastestPrice = cheapestPrice <= SearchModel.to.Length ? SearchModel.to.Length : cheapestPrice + 1;

            int fastestTime = SearchModel.from.Length;
            int cheapestTime = SearchModel.to.Length;


            var myCheapAnser = new TimeAndPrice(cheapestTime, cheapestPrice);
            var myFastAnser = new TimeAndPrice(fastestTime, fastestPrice);


            return new Dictionary<string, TimeAndPrice>
            {
                {"cheapest", myCheapAnser},
                {"fastest", myFastAnser}
            };
        }

        private Dictionary<String, TimeAndPrice> GetResponseFromCalculator(RequestModel SearchModel)
        {
            String toCity = SearchModel.to;
            String fromCity = SearchModel.from;
            CalculatorService cs = new CalculatorService();
            TimeAndPrice timeAndPriceCheap = new TimeAndPrice();
            TimeAndPrice timeAndPriceFast = new TimeAndPrice();

            using (MyEntities db = new MyEntities())
            {
                City startCity = db.Cities.Where(city => city.Name.Replace("\n", "").Replace("\r", "").Equals(fromCity.Replace("\n", "").Replace("\r", ""))).FirstOrDefault();
                City endCity = db.Cities.Where(city => city.Name.Replace("\n", "").Replace("\r", "").Equals(toCity.Replace("\n", "").Replace("\r", ""))).FirstOrDefault();
                List<City> cities = db.Cities.ToList();
                List<Connection> connections = db.Connections.ToList();
                //Route, Price, Hours
                Graph<int, string> cheapGraph = cs.createAndConnectNodes(cities, connections, "Cheapest");
                Tuple<string, double, int> cheapestTuple = cs.getCheapPath(cheapGraph, startCity, endCity);
                Graph<int, string> fastestGraph = cs.createAndConnectNodes(cities, connections, "Fastest");
                Tuple<string, double, int> fastestTuble = cs.getFastPath(fastestGraph, startCity, endCity);

                var cheapPrice = (double)cheapestTuple.Item2;
                var cheapTravelTime = (int) cheapestTuple.Item3;
                timeAndPriceCheap.time = cheapTravelTime;
                timeAndPriceCheap.price = cheapPrice;

                var fastPrice = (double)fastestTuble.Item2;
                var fastTime = (int) cheapestTuple.Item3;
                timeAndPriceFast.time = fastTime;
                timeAndPriceFast.price = fastPrice;
            }
            return new Dictionary<string, TimeAndPrice>
            {
                {"cheapest", timeAndPriceCheap},
                {"fastest", timeAndPriceFast}
            };





        }
    }
}

