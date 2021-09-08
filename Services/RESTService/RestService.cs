using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace exmo_trader_bot_console.Services.RESTService
{
    class RestService: IRestService
    {
        public IObservable<IRestResponse> ResponseStream { get; }

        protected readonly RestClient _client;
        private readonly Method _method;
        
        public RestService(string url, Method method, IObservable<RestRequest> requestStream)
        {
            _method = method;
            _client = new RestClient(url);
            _client.Timeout = -1;

            ResponseStream = requestStream.Select(ExecuteRequest);
        }

        protected virtual IRestResponse ExecuteRequest(RestRequest restRequest)
        {
            return _client.Execute(restRequest, _method);
        }
    }
}
