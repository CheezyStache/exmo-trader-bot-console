using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.Internal;

namespace exmo_trader_bot_console.Services.ParserService.Exmo
{
    class ExmoErrorParserService: BaseOutputStreamService<ErrorResponse>, IErrorParserService
    {
        public void Subscribe(IObservable<string> inputStream)
        {
            OutputStream = inputStream.Select(ParseResponse);
        }

        private ErrorResponse ParseResponse(string response)
        {
            var exmoResponse = JsonSerializer.Deserialize<ExmoSocketResponse>(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var date = ParseDate(exmoResponse.ts);
            var code = exmoResponse.code ?? 0;

            var message = exmoResponse.error;
            if (string.IsNullOrWhiteSpace(message))
                message = exmoResponse.message;

            return new ErrorResponse
            {
                Code = code,
                Date = date,
                Message = message
            };
        }

        private DateTime ParseDate(long date)
        {
            return new DateTime(1970, 01, 01).AddMilliseconds(date);
        }
    }
}
