using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Models
{
    public class CandleProps
    {
        public double UpperShadowPercent { get; set; }
        public double LowerShadowPercent { get; set; }
        public CandleMovement Movement { get; set; }
    }
}
