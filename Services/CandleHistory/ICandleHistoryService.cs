using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Settings;

namespace exmo_trader_bot_console.Services.CandleHistory
{
    interface ICandleHistoryService: IOutputService<CandlesSet>
    {
        void GetCandles();
        void GetCandles(IEnumerable<DataSettings> settings);
    }
}
