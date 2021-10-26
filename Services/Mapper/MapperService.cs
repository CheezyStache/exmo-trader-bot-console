using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using exmo_trader_bot_console.Models.Exmo;

namespace exmo_trader_bot_console.Services.Mapper
{
    class MapperService: IMapperService
    {
        private readonly IMapper _mapper;

        public MapperService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public IObservable<TS> Map<T, TS>(IObservable<T> inputStream)
        {
            return inputStream.Select(input => _mapper.Map<T, TS>(input));
        }

        public IObservable<string> Serialize<T>(IObservable<T> inputStream)
        {
            return inputStream.Select(input => JsonSerializer.Serialize<T>(input,
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}));
        }

        public IObservable<T> Deserialize<T>(IObservable<string> inputStream)
        {
            return inputStream.Select(input => JsonSerializer.Deserialize<T>(input,
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase}));
        }
    }
}
