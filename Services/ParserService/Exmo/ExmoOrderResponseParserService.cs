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
    class ExmoOrderResponseParserService: BaseOutputStreamService<bool>, IOrderResponseParserService
    {
        public void Subscribe(IObservable<string> inputStream)
        {
            OutputStream = inputStream.Select(ParseResponse);
        }

        private bool ParseResponse(string response)
        {
            var createResponse = JsonSerializer.Deserialize<ExmoOrderCreateResponse>(response);

            return createResponse?.Result ?? false;
        }
    }
}
