using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.Internal;

namespace exmo_trader_bot_console.Services.ParserService
{
    public class ExmoEventsParserService: IEventParserService
    {
        public IEnumerable<ResponseWithEvent> ParseResponse(string response)
        {
            return new[] { ParseResponseWithEvent(response) };
        }

        public IObservable<ResponseWithEvent> ParserStream(IObservable<string> responseStream)
        {
            return responseStream.Select(ParseResponseWithEvent);
        }

        private ResponseWithEvent ParseResponseWithEvent(string response)
        {
            var exmoResponse = JsonSerializer.Deserialize<ExmoSocketResponse>(response);
            var responseEvent = ParseEvent(exmoResponse.eventProperty);

            return new ResponseWithEvent(response, responseEvent);
        }

        private ResponseEvent ParseEvent(string eventProperty)
        {
            switch (eventProperty)
            {
                case "update":
                    return ResponseEvent.Update;
                case "snapshot":
                    return ResponseEvent.Snapshot;
                case "info":
                    return ResponseEvent.Info;
                case "logged_in":
                    return ResponseEvent.Login;
                case "subscribed":
                    return ResponseEvent.Subscribe;
                case "unsubscribed":
                    return ResponseEvent.Unsubscribe;
                case "error":
                    return ResponseEvent.Error;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
