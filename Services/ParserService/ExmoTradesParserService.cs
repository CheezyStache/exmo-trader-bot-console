using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.TradingData;
using TraderBot.Models.Exmo;

namespace exmo_trader_bot_console.Services.ParserService
{
    public class ExmoTradesParserService: ITradesParserService
    {
        public IEnumerable<Trade> ParseResponse(string response)
        {
            var exmoResponse = JsonSerializer.Deserialize<ExmoSocketResponse<ExmoTrades[]>>(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            return exmoResponse.data.Select(GetByExmoTrade);
        }

        public IObservable<Trade> ParserStream(IObservable<string> responseStream)
        {
            return responseStream.SelectMany(ParseResponse);
        }

        private Trade GetByExmoTrade(ExmoTrades exmoTrade)
        {
            var date = ParseDate(exmoTrade.date);
            var type = ParseTradeType(exmoTrade.type);
            var price = ParseDouble(exmoTrade.price);
            var quantity = ParseDouble(exmoTrade.quantity);
            var amount = ParseDouble(exmoTrade.amount);

            return new Trade(type, price, quantity, amount, date);
        }

        private DateTime ParseDate(long date)
        {
            date *= 1000;
            return new DateTime(1970, 01, 01).AddMilliseconds(date);
        }

        private TradeType ParseTradeType(string type)
        {
            switch (type)
            {
                case "buy":
                    return TradeType.Buy;
                case "sell":
                    return TradeType.Sell;

                default:
                    throw new NotImplementedException();
            }
        }

        private double ParseDouble(string number)
        {
            return Convert.ToDouble(number, CultureInfo.InvariantCulture);
        }
    }
}
