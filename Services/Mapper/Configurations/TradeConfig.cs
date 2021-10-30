using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Utils;
using TraderBot.Models.Exmo;

namespace exmo_trader_bot_console.Services.Mapper.Configurations
{
    class TradeConfig
    {
        public TradeConfig(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ExmoSocketResponse<ExmoTrades[]>, IEnumerable<Trade>>()
                .ConstructUsing(src => src.data.Select(d => GetByExmoTrade(d, src.topic)));
        }

        private Trade GetByExmoTrade(ExmoTrades exmoTrade, string topic)
        {
            var date = DateUtils.GetDate(exmoTrade.date);
            var type = ParseTradeType(exmoTrade.type);
            var price = NumberUtils.ParseDouble(exmoTrade.price);
            var quantity = NumberUtils.ParseDouble(exmoTrade.quantity);
            var amount = NumberUtils.ParseDouble(exmoTrade.amount);
            var pair = ParsePair(topic);

            return new Trade(type, price, quantity, amount, date, pair);
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
                    return TradeType.Buy;
                case "market_sell":
                    return TradeType.Sell;
                case "market_buy_total":
                    return TradeType.Buy;
                case "market_sell_total":
                    return TradeType.Sell;

                default:
                    throw new NotImplementedException();
            }
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
