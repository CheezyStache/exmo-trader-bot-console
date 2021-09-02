using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.DataStorageService
{
    public interface IDataStorageService
    {
        IObservable<Trade> TradesStream { get; }
        IObservable<Trade> OrdersStream { get; }

        ICollection<Trade> TradesCollection { get; }
        ICollection<Trade> OrdersCollection { get; }

        void AddTrade(Trade trade);
        void AddOrder(Trade order);
    }
}
