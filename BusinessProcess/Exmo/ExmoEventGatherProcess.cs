using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.ParserService.Exmo;
using exmo_trader_bot_console.Services.WebSocketService;

namespace exmo_trader_bot_console.BusinessProcess.Exmo
{
    class ExmoEventGatherProcess: IEventGatherProcess
    {
        public IObservable<ResponseWithEvent> EventsStream { get; }

        public ExmoEventGatherProcess(Settings settings)
        {
            IDataWebSocketService dataWebSocketService = new ExmoDataWebSocketService(settings);
            IEventParserService eventParserService = new ExmoEventsParserService();

            var webSocketStream = dataWebSocketService.ConnectToApi(APIType.Public);
            EventsStream = eventParserService.ParserStream(webSocketStream);
        }
    }
}
