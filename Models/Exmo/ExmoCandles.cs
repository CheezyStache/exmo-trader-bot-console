using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Models.Exmo
{
    public class ExmoCandleSet
    {
        public ExmoCandle[] Candles { get; set; }
        public TradingPair Pair { get; set; }
        public int Resolution { get; set; }
    }

    public class ExmoCandle
    {
        public long T { get; set; }
        public double O { get; set; }
        public double C { get; set; }
        public double H { get; set; }
        public double L { get; set; }
        public double V { get; set; }
    }
}
