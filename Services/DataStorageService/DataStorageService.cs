using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.DataStorageService
{
    public class DataStorageService: IDataStorageService
    {
        public IObservable<Trade> TradesStream => _tradesSubject;
        public IObservable<Trade> OrdersStream => _ordersSubject;
        public ICollection<Trade> TradesCollection { get; }
        public ICollection<Trade> OrdersCollection { get; }

        private readonly ISubject<Trade> _tradesSubject;
        private readonly ISubject<Trade> _ordersSubject;

        public DataStorageService()
        {
            _tradesSubject = new Subject<Trade>();
            _ordersSubject = new Subject<Trade>();

            TradesCollection = new List<Trade>();
            OrdersCollection = new List<Trade>();
        }

        public void AddTrade(Trade trade)
        {
            TradesCollection.Add(trade);
            _tradesSubject.OnNext(trade);
        }

        public void AddOrder(Trade order)
        {
            OrdersCollection.Add(order);
            _ordersSubject.OnNext(order);
        }
    }
}
