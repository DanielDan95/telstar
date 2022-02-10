using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dijkstra.NET.Graph;
using Dijkstra.NET.ShortestPath;

namespace telstarapp.Services
{
    public class CalculatorService
    {

        public int shortestRoute(List<City> cities, List<Connection> connection)
        {
            var graph = new Graph<int, string>();

            foreach (int node in nodes)
                graph.AddNode(node);

            graph.Connect()



            return 0;
        }

        public int cheapestRoute(List<City> cities, List<Connection> connection)
        {
            return 0;
        }



    }
}