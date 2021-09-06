using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;

namespace exmo_trader_bot_console.BusinessProcess
{
    interface IEventGatherProcess
    {
        IObservable<ResponseWithEvent> EventsStream { get; }
    }
}
