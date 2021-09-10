using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;
using RestSharp;

namespace exmo_trader_bot_console.Services.RequestService
{
    class ExmoOrderRequestService: IOrderRequestService
    {
        public IObservable<RestRequest> RequestStream(IObservable<OrderDecision> requestStream)
        {
            return requestStream.Select(SerializeRequest);
        }

        private RestRequest SerializeRequest(OrderDecision orderDecision)
        {
            var type = SerializeTradeType(orderDecision.Type);
            var pair = SerializePair(orderDecision.Pair);

            var request = new RestRequest();
            request.AddParameter("pair", pair);
            request.AddParameter("quantity", orderDecision.Quantity.ToString(CultureInfo.InvariantCulture));
            request.AddParameter("price", orderDecision.Price.ToString(CultureInfo.InvariantCulture));
            request.AddParameter("type", type);

            return request;
        }

        private string SerializeTradeType(TradeType type)
        {
            switch (type)
            {
                case TradeType.Buy:
                    return "buy";
                case TradeType.Sell:
                    return "sell";
                case TradeType.MarketBuyPrice:
                    return "market_buy";
                case TradeType.MarketSellPrice:
                    return "market_sell";
                case TradeType.MarketBuyQuantity:
                    return "market_buy_total";
                case TradeType.MarketSellQuantity:
                    return "market_sell_total";

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
