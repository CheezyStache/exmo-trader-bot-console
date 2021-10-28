using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.Services.Settings;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.SettingsService
{
    class CandleSettingsService : SettingsService<CandleSignalsSettings>
    {
        public CandleSettingsService() : base("candlesSettings.json") { }
    }
}
