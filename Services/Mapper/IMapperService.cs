using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.Mapper
{
    interface IMapperService
    {
        IObservable<TS> Map<T, TS>(IObservable<T> inputStream);
        IObservable<string> Serialize<T>(IObservable<T> inputStream);
        IObservable<T> Deserialize<T>(IObservable<string> inputStream);
    }
}
