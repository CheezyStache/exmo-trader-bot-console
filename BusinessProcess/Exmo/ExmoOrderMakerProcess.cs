using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.ParserService.Exmo;
using exmo_trader_bot_console.Services.RequestService;
using RestSharp;

namespace exmo_trader_bot_console.BusinessProcess.Exmo
{
    class ExmoOrderMakerProcess: IOrderMakerProcess
    {
        public IObservable<OrderResult> OrderResultStream { get; }

        public ExmoOrderMakerProcess(IObservable<OrderDecision> decisionsStream, Settings settings)
        {
            //IOrderRequestService orderRequestService = new ExmoOrderRequestService();
            //var requests = orderRequestService.RequestStream(decisionsStream);

            ////IRestService restService = new ExmoRestService(settings);
            ////var responses = restService.ResponseStream(requests);

            //var responses = requests.Select(r => new RestResponse
            //{
            //    Content = JsonSerializer.Serialize(new ExmoOrderCreateResponse {Result = true},
            //        new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase})
            //}); // for testing

            //var orderResponseParser = new ExmoOrderResponseParserService();
            //var orderResponses = orderResponseParser.ParserStream(responses.Select(r => r.Content));

            //OrderResultStream = decisionsStream.Zip(orderResponses, ConstructOrderResult);
        }

        private OrderResult ConstructOrderResult(OrderDecision decision, bool response)
        {
            OrderResult orderResult = decision as OrderResult;
            orderResult.Result = response;
            orderResult.Date = DateTime.Now;
            return orderResult;
        }
    }
}
