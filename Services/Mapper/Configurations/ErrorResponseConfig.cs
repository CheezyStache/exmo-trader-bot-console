using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Utils;

namespace exmo_trader_bot_console.Services.Mapper.Configurations
{
    class ErrorResponseConfig
    {
        public ErrorResponseConfig(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ExmoSocketResponse, ErrorResponse>()
                .ForMember(dest => dest.Date,
                    opt => opt.MapFrom(src => DateUtils.GetDate(src.ts)))
                .ForMember(dest => dest.Code,
                    opt => opt.MapFrom(src => src.code ?? 0))
                .ForMember(dest => dest.Message,
                    opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.error) ? src.message : src.error));
        }
    }
}
