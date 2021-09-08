using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.EventRouterService;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.ParserService.Exmo;

namespace exmo_trader_bot_console.BusinessProcess.Exmo
{
    class ExmoDataStorageFillerProcess: IDataStorageFillerProcess
    {
        public IObservable<Trade> TradesStream { get; }

        public ExmoDataStorageFillerProcess(IObservable<ResponseWithEvent> eventsStream)
        {
            IEventRouterService eventRouterService = new TradesEventRouterService(eventsStream);
            ITradesParserService tradesParserService = new ExmoTradesParserService();
            IDataStorageService dataStorageService = new DataStorageService();

            TradesStream = dataStorageService.TradesStream;

            var updatesStream = eventRouterService.EventStream
                .Select(e => e.Response);

            tradesParserService.ParserStream(updatesStream)
                .Subscribe(dataStorageService.AddTrade);
        }
    }
}
