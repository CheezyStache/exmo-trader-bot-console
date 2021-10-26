using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Utils;

namespace exmo_trader_bot_console.Services.Mapper.Configurations
{
    class OrderResultConfig
    {
        public OrderResultConfig(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ExmoSocketResponse<ExmoUserTrades>, OrderResult>()
                .ForMember(dest => dest.Amount,
                    opt => opt.MapFrom(src => NumberUtils.ParseDouble(src.data.amount)))
                .ForMember(dest => dest.CommissionAmount,
                    opt => opt.MapFrom(src => NumberUtils.ParseDouble(src.data.commission_amount)))
                .ForMember(dest => dest.CommissionCurrency,
                    opt => opt.MapFrom(src => NumberUtils.ParseDouble(src.data.commission_currency)))
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => DateUtils.GetDate(src.data.date)))
                .ForMember(dest => dest.ExecType,
                    opt => opt.MapFrom(src => ParseTradeExecType(src.data.exec_type)))
                .ForMember(dest => dest.Pair,
                    opt => opt.MapFrom(src => ParsePair(src.data.pair)))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => NumberUtils.ParseDouble(src.data.price)))
                .ForMember(dest => dest.Quantity,
                    opt => opt.MapFrom(src => NumberUtils.ParseDouble(src.data.quantity)))
                .ForMember(dest => dest.Type,
                    opt => opt.MapFrom(src => ParseTradeType(src.data.type)));
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
