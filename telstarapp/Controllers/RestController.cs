using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using telstarapp.Models;
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
           [FromUri] Boolean refrigerated = false
           )
        {
            if (!isValidInput(from, to, weight, size, recommended, weapon, cautious, refrigerated))
            {
                return NotFound();
            }
            var model = new RequestModel(size, from, to, weight, recommended, weapon, cautious, refrigerated);
            var responseModel = GetResponse(model);
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

        private bool isValidInput(String from, String to, Double weight, Dictionary<string, int> size, Boolean recommended, Boolean weapon, Boolean cautious, Boolean refrigerated)
        {
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
    }
}

