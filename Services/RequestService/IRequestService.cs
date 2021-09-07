using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace exmo_trader_bot_console.Services.RequestService
{
    interface IRequestService<in T> where T : class
    {
        IObservable<RestRequest> RequestStream(IObservable<T> requestStream);
    }
}
