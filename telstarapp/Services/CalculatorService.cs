using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;
using telstarapp.Models;

namespace telstarapp.Services
{
    public class CalculatorService
    {

        public Graph<int, string> createAndConnectNodes(List<City> cities, List<Connection> connection, string weight)
        {
            int price;
            var graph = new Graph<int, string>();
            for (int i = 0; i < cities.Count+4; i++)
            {
                graph.AddNode(i);
            }
            foreach (Connection con in connection)
            {
                
                String owner = "";
                int time = 0;
                // Converting int? to uint to work with the Dijkstra.NET
                int? castCity1 = con.City1;
                uint castedCity1 = Convert.ToUInt32(castCity1);
                int? castCity2 = con.City2;
                uint castedCity2 = Convert.ToUInt32(castCity2);
                int? castHours = con.TimeOfOneSegmentInHours;
                int castedHours = Convert.ToInt32(castHours);

                if (con.Owner == 0)
                {
                    owner = "Telstar Logistics";
                    time = (int)(con.NumberOfSegments * con.TimeOfOneSegmentInHours); // changing datatype from int? to int, and calulating the hours
                    price = (int)Math.Ceiling((double)(con.NumberOfSegments * Math.Ceiling((double)con.PriceOfOneSegment))); // changing datatype from double? to int and calcuating the price
                    if(weight == "Cheapest")
                    {
                        graph.Connect(castedCity1, castedCity2, price, "edge between " + con.City1 + " and " + con.City2 + " and it's owned by " + owner);
                        graph.Connect(castedCity2, castedCity1, price, "edge between " + con.City1 + " and " + con.City2 + " and it's owned by " + owner);
                    } else
                    {
                        graph.Connect(castedCity1, castedCity2, time, "edge between " + con.City1 + " and " + con.City2 + " and it's owned by " + owner);
                        graph.Connect(castedCity2, castedCity1, time, "edge between " + con.City1 + " and " + con.City2 + " and it's owned by " + owner);
                    }

                }
                else if (con.Owner == 1)
                {
                    owner = "Oceanic Alliance";

                } else if(con.Owner == 2){
                    owner = "EIT";

                } else
                {
                    owner = "Unknown";
                }
                





            }
            return graph;
        }
        public Tuple<string, double, int> getCheapPath(Graph<int, string> graph, City city1, City city2)
        {
            String route = "";
            double price;
            int time;
            Console.WriteLine(city1.Name);
            int? castCity1 = city1.CityID;
            uint castedCity1 = Convert.ToUInt32(castCity1);
            int? castCity2 = city2.CityID;
            uint castedCity2 = Convert.ToUInt32(castCity2);
            
            ShortestPathResult result = graph.Dijkstra(castedCity1, castedCity2); //if break here spelling mistake
            var path = result.GetPath();

            price = result.Distance; //price should be changed, on the todo list. 3 is 75% 4.
            time = result.Distance/3*4; // time should be changed, on the todo list
            foreach (var item in path)
            {
 
                route += " " + item;
                // get access to database
            }
            

            var pathing = new Tuple<string, double, int>(route, price, time);

            return pathing;
        }
        public Tuple<string, double, int> getFastPath(Graph<int, string> graph, City city1, City city2)
        {
            String route = "";
            double price;
            int time;
            Console.WriteLine(city1.Name);
            int? castCity1 = city1.CityID;
            uint castedCity1 = Convert.ToUInt32(castCity1);
            int? castCity2 = city2.CityID;
            uint castedCity2 = Convert.ToUInt32(castCity2);

            ShortestPathResult result = graph.Dijkstra(castedCity1, castedCity2); //if break here spelling mistake
            var path = result.GetPath();

            price = result.Distance * 0.75; //price should be changed, on the todo list. 3 is 75% 4.
            time = result.Distance; // time should be changed, on the todo list
            foreach (var item in path)
            {

                route += " " + item;
                // get access to database
            }


            var pathing = new Tuple<string, double, int>(route, price, time);

            return pathing;
        }
        //Cheapest is the exact same as the one above, because we are not able to get answers from all our competitors


    }
}