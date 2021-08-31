using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.ParserService
{
    public class ParserService: IParserService
    {
        public T ParseResponse<T>(string response) where T : class
        {
            return JsonSerializer.Deserialize<T>(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        }

        public IObservable<T> ParserStream<T>(IObservable<string> responseStream) where T : class
        {
            return responseStream.Select(ParseResponse<T>);
        }
    }
}
