using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace telstarapp.Models
{
    public class RequestModel
    {
        public RequestModel(Dictionary<string, int> size, string from, string to, double weight, bool recommended, bool weapon, bool cautious, bool refrigerated)
        {
            this.size = size;
            this.from = from;
            this.to = to;
            this.weight = weight;
            this.recommended = recommended;
            this.weapon = weapon;
            this.cautious = cautious;
            this.refrigerated = refrigerated;
        }

        public Dictionary<string, int> size { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public double weight { get; set; }
        public bool recommended { get; set; }
        public bool weapon { get; set; }
        public bool cautious { get; set; }
        public bool refrigerated { get; set; }
    }
    public class TimeAndPrice
    {
        public TimeAndPrice(int time, double price)
        {
            this.time = time;
            this.price = price;
        }

        public TimeAndPrice()
        {
            
        }

        public int time { get; set; }
        public double price { get; set; }

    }
}