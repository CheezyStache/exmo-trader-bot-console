using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Models.OrderData
{
    public class OrderDecision
    {
        public TradingPair Pair { get; set; }
        public TradeType Type { get; set; }
        public int Resolution { get; set; }
    }
}
