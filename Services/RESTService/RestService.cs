using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace exmo_trader_bot_console.Services.RESTService
{
    class RestService: BaseOutputStreamService<IRestResponse>, IRestService
    {
        protected readonly RestClient _client;
        private readonly Method _method;
        
        public RestService(string url, Method method)
        {
            _method = method;
            _client = new RestClient(url);
            _client.Timeout = -1;
        }

        public void Subscribe(IObservable<IRestRequest> inputStream)
        {
            OutputStream = inputStream.Select(ExecuteRequest);
        }

        protected virtual IRestResponse ExecuteRequest(IRestRequest restRequest)
        {
            return _client.Execute(restRequest, _method);
        }
    }
}
