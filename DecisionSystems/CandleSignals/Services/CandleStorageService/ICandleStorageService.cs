using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.CandleStorageService
{
    interface ICandleStorageService: IStreamService<Trade, Trade[][]>
    {
    }
}
