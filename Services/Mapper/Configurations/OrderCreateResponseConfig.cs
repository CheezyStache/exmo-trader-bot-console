using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using exmo_trader_bot_console.Models.Exmo;

namespace exmo_trader_bot_console.Services.Mapper.Configurations
{
    class OrderCreateResponseConfig
    {
        public OrderCreateResponseConfig(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<ExmoOrderCreateResponse, bool>().ConvertUsing(src => src.Result);
        }
    }
}
