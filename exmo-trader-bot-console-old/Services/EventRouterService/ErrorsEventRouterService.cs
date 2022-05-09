using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;

namespace exmo_trader_bot_console.Services.EventRouterService
{
    class ErrorsEventRouterService: BaseOutputStreamService<string>, IErrorsEventRouterService
    {
        public void Subscribe(IObservable<ResponseWithEvent> inputStream)
        {
            OutputStream = inputStream.Where(e => e.Event == ResponseEvent.Error)
                .Select(e => e.Response);
        }
    }
}
