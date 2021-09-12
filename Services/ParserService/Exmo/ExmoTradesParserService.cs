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

namespace exmo_trader_bot_console.Services.ParserService.Exmo
{
    public class ExmoTradesParserService: BaseOutputStreamService<Trade>, ITradesParserService
    {
        public void Subscribe(IObservable<string> inputStream)
        {
            OutputStream = inputStream.SelectMany(ParseResponse);
        }

        private IEnumerable<Trade> ParseResponse(string response)
        {
            var exmoResponse = JsonSerializer.Deserialize<ExmoSocketResponse<ExmoTrades[]>>(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var topic = exmoResponse.topic;

            return exmoResponse.data.Select(d => GetByExmoTrade(d, topic));
        }

        private Trade GetByExmoTrade(ExmoTrades exmoTrade, string topic)
        {
            var date = ParseDate(exmoTrade.date);
            var type = ParseTradeType(exmoTrade.type);
            var price = ParseDouble(exmoTrade.price);
            var quantity = ParseDouble(exmoTrade.quantity);
            var amount = ParseDouble(exmoTrade.amount);
            var pair = ParsePair(topic);

            return new Trade(type, price, quantity, amount, date, pair);
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
                case "market_buy":
                    return TradeType.MarketBuyPrice;
                case "market_sell":
                    return TradeType.MarketSellPrice;
                case "market_buy_total":
                    return TradeType.MarketBuyQuantity;
                case "market_sell_total":
                    return TradeType.MarketSellQuantity;

                default:
                    throw new NotImplementedException();
            }
        }

        private double ParseDouble(string number)
        {
            return Convert.ToDouble(number, CultureInfo.InvariantCulture);
        }

        private TradingPair ParsePair(string topic)
        {
            var dotsIndex = topic.IndexOf(':');
            var pair = topic.Substring(dotsIndex + 1).Split('_');

            var tradingPair = new TradingPair
            {
                Crypto = pair[0],
                Currency = pair[1]
            };

            return tradingPair;
        }
    }
}
