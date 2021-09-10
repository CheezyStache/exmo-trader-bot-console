using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Models
{
    public class CandleSignalsSettings
    {
        public double ErrorPercent { get; set; }
        public int MinTrades { get; set; }
        public CandlePattern[] Patterns { get; set; }
    }
}
