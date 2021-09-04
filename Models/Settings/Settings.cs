using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;

namespace exmo_trader_bot_console.Models.Settings
{
    public class Settings
    {
        public PlatformApiSettings Api { get; set; }
        public TradingPairSettings[] Pairs { get; set; }
        public CandleSignalsSystemSettings CandleSystem { get; set; }
    }
}
