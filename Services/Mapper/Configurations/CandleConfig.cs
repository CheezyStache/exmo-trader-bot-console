using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Utils;

namespace exmo_trader_bot_console.Services.Mapper.Configurations
{
    class CandleConfig
    {
        public CandleConfig(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ExmoCandleSet, CandlesSet>()
                .ForMember(dest => dest.Candles,
                    opt => opt.MapFrom(src => src.Candles));

            cfg.CreateMap<ExmoCandle, Candle>()
                .ForMember(dest => dest.Time,
                    opt => opt.MapFrom(src => DateUtils.GetDate(src.T / 1000)))
                .ForMember(dest => dest.Open,
                    opt => opt.MapFrom(src => src.O))
                .ForMember(dest => dest.Close,
                    opt => opt.MapFrom(src => src.C))
                .ForMember(dest => dest.High,
                    opt => opt.MapFrom(src => src.H))
                .ForMember(dest => dest.Low,
                    opt => opt.MapFrom(src => src.L));
        }
    }
}
