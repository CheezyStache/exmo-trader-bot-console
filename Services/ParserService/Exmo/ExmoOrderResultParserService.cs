using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.ParserService.Exmo
{
    class ExmoOrderResultParserService: BaseOutputStreamService<OrderResult>, IOrderResultParserService
    {
        public void Subscribe(IObservable<string> inputStream)
        {
            OutputStream = inputStream.Select(ParseResponse);
        }

        private OrderResult ParseResponse(string response)
        {
            var exmoResponse = JsonSerializer.Deserialize<ExmoSocketResponse<ExmoUserTrades>>(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            var topic = exmoResponse.topic;

            return GetByExmoUserTrade(exmoResponse.data);
        }

        private OrderResult GetByExmoUserTrade(ExmoUserTrades exmoUserTrade)
        {
            var date = ParseDate(exmoUserTrade.date);
            var type = ParseTradeType(exmoUserTrade.type);
            var price = ParseDouble(exmoUserTrade.price);
            var quantity = ParseDouble(exmoUserTrade.quantity);
            var amount = ParseDouble(exmoUserTrade.amount);
            var pair = ParsePair(exmoUserTrade.pair);
            var commissionAmount = ParseDouble(exmoUserTrade.commission_amount);
            var execType = ParseTradeExecType(exmoUserTrade.exec_type);

            return new OrderResult
            {
                Amount = amount,
                CommissionAmount = commissionAmount,
                CommissionCurrency = exmoUserTrade.commission_currency,
                Date = date,
                ExecType = execType,
                Pair = pair,
                Price = price,
                Quantity = quantity,
                Type = type
            };
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

        private TradingPair ParsePair(string pairString)
        {
            var pair = pairString.Split('_');

            var tradingPair = new TradingPair
            {
                Crypto = pair[0],
                Currency = pair[1]
            };

            return tradingPair;
        }

        private OrderExecType ParseTradeExecType(string type)
        {
            switch (type)
            {
                case "taker":
                    return OrderExecType.Taker;
                case "maker":
                    return OrderExecType.Maker;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
