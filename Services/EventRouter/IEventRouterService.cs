using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;

namespace exmo_trader_bot_console.Services.EventRouter
{
    interface IEventRouterService
    {
        void Subscribe(IObservable<ResponseWithEvent> inputStream, string collectionName);
        IObservable<string> GetRouterStream(string collectionName, ResponseEvent responseEvent);
    }
}
