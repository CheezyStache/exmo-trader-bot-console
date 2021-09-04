using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DataStorageService
{
    interface IDataStorageService
    {
        Trade[][] TradeCandles { get; }
        void ConnectToTrades(IObservable<Trade> tradesStream);
        IObservable<Trade[][]> TradeCandlesStream { get; }
    }
}
