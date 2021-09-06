using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.WebSocketService;

namespace exmo_trader_bot_console.BusinessProcess.Exmo
{
    class ExmoEventGatherProcess: EventGatherProcess, IEventGatherProcess
    {
        public ExmoEventGatherProcess(Settings settings)
        {
            var dataWebSocketService = new ExmoDataWebSocketService(settings);
            var eventParserService = new ExmoEventsParserService();

            ConstructProcess(dataWebSocketService, eventParserService);
        }
    }
}
