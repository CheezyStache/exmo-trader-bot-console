using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.EventRouterService;
using exmo_trader_bot_console.Services.ParserService.Exmo;

namespace exmo_trader_bot_console.BusinessProcess.Exmo
{
    class ExmoDataStorageFillerProcess: DataStorageFillerProcess, IDataStorageFillerProcess
    {
        public ExmoDataStorageFillerProcess(IObservable<ResponseWithEvent> eventsStream)
        {
            var eventRouterService = new TradesEventRouterService(eventsStream);
            var tradesParserService = new ExmoTradesParserService();
            var dataStorageService = new DataStorageService();

            ConstructProcess(eventRouterService, tradesParserService, dataStorageService);
        }
    }
}
