using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Models.Settings
{
    public class DataSettings
    {
        public TradingPair Pair { get; set; }
        public double CurrencyAmount { get; set; }
        public double MinDiff { get; set; }

        public ChartSettings[] Chart { get; set; }
    }

    public class ChartSettings
    {
        public int Resolution { get; set; }
        public int StartCandles { get; set; }
    }
}
