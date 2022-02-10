using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using telstarapp.Models;

namespace telstarapp.theSystem.Connector
{
    public class IntegrationService
    {
        private const string URL = "http://wa-oa-dk1.azurewebsites.net/";

        public async Task<TimeAndPrice> GetTimeAndPrice()
        {
            TimeAndPrice timeAndPrice = new TimeAndPrice();

            using (var client = new HttpClient())
            {
                //Passing service base url
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Clear();
                //Define request data format
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //Sending request to find web api REST service resource GetAllEmployees using HttpClient
                HttpResponseMessage Res = await client.GetAsync("api/getRoute?from=hey&to=yb&weight=22.2&recommended=false&cautious=false&refrigerated=false&weapon=true&height=12&width=25&length=2");
                //Checking the response is successful or not which is sent using HttpClient
                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api
                    var TimeAndPriceResponse = Res.Content.ReadAsStringAsync().Result;
                    //Deserializing the response recieved from web api and storing into the Employee list
                    timeAndPrice = JsonConvert.DeserializeObject<TimeAndPrice>(TimeAndPriceResponse);
                }
                //returning the employee list to view
                return timeAndPrice;
            }
        }
    }
}