using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Models.Settings
{
    public class TradingPairSettings
    {
        public TradingPair TradingPair { get; set; }
        public double CurrencyAmount { get; set; }
    }
}
