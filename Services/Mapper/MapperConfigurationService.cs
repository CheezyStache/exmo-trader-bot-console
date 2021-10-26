using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using exmo_trader_bot_console.Services.Mapper.Configurations;

namespace exmo_trader_bot_console.Services.Mapper
{
    public class MapperConfigurationService
    {
        private readonly MapperConfiguration _config;

        public MapperConfigurationService()
        {
            _config = new MapperConfiguration(cfg =>
            {
                new ErrorResponseConfig(cfg);
                new OrderCreateResponseConfig(cfg);
                new TradeConfig(cfg);
                new ResponseWithEventConfig(cfg);
                new OrderResultConfig(cfg);
            });
        }

        public IMapper CreateMapper()
        {
            return _config.CreateMapper();
        }
    }
}
