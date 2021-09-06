using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;

namespace exmo_trader_bot_console.Services.EventRouterService
{
    class TradesEventRouterService: IEventRouterService
    {
        public IObservable<ResponseWithEvent> EventStream { get; }

        public TradesEventRouterService(IObservable<ResponseWithEvent> eventStream)
        {
            EventStream = eventStream.Where(e => e.Event == ResponseEvent.Update);
        }
    }
}
