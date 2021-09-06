using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.WebSocketService;

namespace exmo_trader_bot_console.BusinessProcess
{
    abstract class EventGatherProcess
    {
        public IObservable<ResponseWithEvent> EventsStream { get; private set; }

        protected void ConstructProcess(IDataWebSocketService dataWebSocketService,
            IEventParserService eventParserService)
        {
            if (EventsStream != null)
                throw new Exception("Event gather process has been already constructed");

            var webSocketStream = dataWebSocketService.ConnectToApi(APIType.Public);
            EventsStream = eventParserService.ParserStream(webSocketStream);
        }
    }
}
