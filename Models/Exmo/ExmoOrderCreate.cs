using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Exmo
{
    class ExmoOrderCreate
    {
        public ExmoOrderCreate(string pair, double quantity, double price, string type)
        {
            Pair = pair;
            Quantity = quantity;
            Price = price;
            Type = type;
        }

        public string Pair { get; set; }
        public double Quantity { get; set; }
        public double Price { get; set; }
        public string Type { get; set; }
        public string Client_id { get; set; }
    }
}
