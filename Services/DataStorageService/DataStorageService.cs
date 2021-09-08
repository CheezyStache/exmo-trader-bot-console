using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.DataStorageService
{
    public class DataStorageService: IDataStorageService
    {
        public IObservable<Trade> TradesStream => _tradesSubject;
        public IObservable<OrderDecision> OrdersStream => _ordersSubject;

        private readonly ISubject<Trade> _tradesSubject;
        private readonly ISubject<OrderDecision> _ordersSubject;

        public DataStorageService()
        {
            _tradesSubject = new Subject<Trade>();
            _ordersSubject = new Subject<OrderDecision>();
        }

        public void AddTrade(Trade trade)
        {
            _tradesSubject.OnNext(trade);
        }

        public void AddOrder(OrderDecision order)
        {
            _ordersSubject.OnNext(order);
        }
    }
}
