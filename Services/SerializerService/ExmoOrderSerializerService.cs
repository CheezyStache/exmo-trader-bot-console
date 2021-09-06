using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.SerializerService
{
    class ExmoOrderSerializerService: IOrderSerializerService
    {
        public string SerializeRequest(OrderDecision request)
        {
            var type = SerializeTradeType(request.Type);
            var pair = SerializePair(request.Pair);

            var exmoOrderCreate = new ExmoOrderCreate(pair, request.Quantity, request.Price, type);

            return JsonSerializer.Serialize(exmoOrderCreate,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
        }

        public IObservable<string> SerializerStream(IObservable<OrderDecision> requestStream)
        {
            return requestStream.Select(SerializeRequest);
        }

        private string SerializeTradeType(TradeType type)
        {
            switch (type)
            {
                case TradeType.Buy:
                    return "buy";
                case TradeType.Sell:
                    return "sell";
                case TradeType.MarketBuy:
                    return "market_buy";
                case TradeType.MarketSell:
                    return "market_sell";

                default:
                    throw new NotImplementedException();
            }
        }

        private string SerializePair(TradingPair pair)
        {
            return $"{pair.Crypto}_{pair.Currency}";
        }
    }
}
