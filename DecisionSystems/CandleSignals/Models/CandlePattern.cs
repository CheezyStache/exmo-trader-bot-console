using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Models
{
    public class CandlePattern
    {
        public string Name { get; set; }
        public CandleSignal Signal { get; set; }
        public CandleProps[] Candles { get; set; }
    }
}
