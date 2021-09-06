using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.RESTService
{
    class RestService: IRestService
    {
        private readonly HttpClient _client;
        
        public RestService(HttpClient client)
        {
            _client = client;
        }

        public IObservable<HttpResponseMessage> Get(string url)
        {
            return Observable.FromAsync(() => _client.GetAsync(url));
        }

        public IObservable<HttpResponseMessage> Post(string url, HttpContent content)
        {
            return Observable.FromAsync(() => _client.PostAsync(url, content));
        }
    }
}
