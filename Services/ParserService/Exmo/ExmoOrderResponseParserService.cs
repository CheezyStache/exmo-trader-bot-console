using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;

namespace exmo_trader_bot_console.Services.ParserService.Exmo
{
    class ExmoOrderResponseParserService: IOrderResponseParserService
    {
        public IObservable<bool> ParserStream(IObservable<string> responseStream)
        {
            return responseStream.Select(ParseResponse);
        }

        public bool ParseResponse(string response)
        {
            return JsonSerializer.Deserialize<ExmoOrderCreateResponse>(response)
                .Result;
        }
    }
}
