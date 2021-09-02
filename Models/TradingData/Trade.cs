using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.TradingData
{
    public class Trade
    {
        public Trade(TradeType type, double price, double quantity,
            double amount, DateTime dateTime)
        {
            Type = type;
            Price = price;
            Quantity = quantity;
            Amount = amount;
            DateTime = dateTime;
        }

        public TradeType Type { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public double Amount { get; set; }
        public DateTime DateTime { get; set; }
    }
}
