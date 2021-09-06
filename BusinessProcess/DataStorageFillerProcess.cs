using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.EventRouterService;
using exmo_trader_bot_console.Services.ParserService;

namespace exmo_trader_bot_console.BusinessProcess
{
    abstract class DataStorageFillerProcess
    {
        public IObservable<Trade> TradesStream { get; private set; }

        protected void ConstructProcess(IEventRouterService eventRouterService,
            ITradesParserService tradesParserService,
            IDataStorageService dataStorageService)
        {
            TradesStream = dataStorageService.TradesStream;

            var updatesStream = eventRouterService.EventStream
                .Select(e => e.Response);

            tradesParserService.ParserStream(updatesStream)
                .Subscribe(dataStorageService.AddTrade);
        }
    }
}
