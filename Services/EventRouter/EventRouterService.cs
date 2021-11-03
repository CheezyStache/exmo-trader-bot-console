using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;

namespace exmo_trader_bot_console.Services.EventRouter
{
    public class EventRouterService: IEventRouterService
    {
        private readonly IDictionary<string, IObservable<ResponseWithEvent>> _responseDictionary;

        public EventRouterService()
        {
            _responseDictionary = new Dictionary<string, IObservable<ResponseWithEvent>>();
        }

        public void Subscribe(IObservable<ResponseWithEvent> inputStream, string collectionName)
        {
            var result = _responseDictionary.TryGetValue(collectionName, out _);
            if (result)
                throw new ArgumentException("This collection is already exists in Event Router Service");

            _responseDictionary.Add(collectionName, inputStream);
        }

        public IObservable<string> GetRouterStream(string collectionName, ResponseEvent responseEvent)
        {
            var result = _responseDictionary.TryGetValue(collectionName, out _);
            if(!result)
                throw new ArgumentException("This collection does not exist in Event Router Service");

            return _responseDictionary[collectionName].Where(e => e.Event == responseEvent)
                .Select(e => e.Response);
        }
    }
}
