using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.DataStorage
{
    public interface IDataStorageService: IInputService<CandlesSet>
    {
        IObservable<Candle[]> GetCandles(TradingPair pair, int resolution);
    }
}
