using System;
using System.Linq;
using System.Reactive.Linq;
using exmo_trader_bot_console.BusinessProcess;
using exmo_trader_bot_console.BusinessProcess.Exmo;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DataStorageService;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Services.ParserService.Exmo;
using exmo_trader_bot_console.Services.RequestService;
using exmo_trader_bot_console.Services.RESTService;
using exmo_trader_bot_console.Services.SettingsService;

namespace exmo_trader_bot_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Trader Bot is started!");
            var settingsService = new SettingsService();
            var settings = settingsService.GetSettings();

            IEventGatherProcess eventGatherProcess = new ExmoEventGatherProcess(settings);
            IDataStorageFillerProcess dataStorageFillerProcess =
                new ExmoDataStorageFillerProcess(eventGatherProcess.EventsStream);

            dataStorageFillerProcess.TradesStream.Subscribe(Console.WriteLine);

            IDataStorageService dataStorageService = new DataStorageService(settings);
            dataStorageService.ConnectToTrades(dataStorageFillerProcess.TradesStream);
            IDecisionService decisionService = new CandleSignalsDecisionService(dataStorageService.TradeCandlesStream);
            IOrderRequestService orderRequestService = new ExmoOrderRequestService();
            var requests = orderRequestService.RequestStream(decisionService.DecisionsStream);
            IRestService restService = new ExmoRestService(settings);
            var responses = restService.ResponseStream(requests);
            var orderResponseParser = new ExmoOrderResponseParserService();
            var orderResponses = orderResponseParser.ParserStream(responses.Select(r => r.Content));
            var orderStream = decisionService.DecisionsStream.CombineLatest(orderResponses, (request, response) =>
            {
                OrderResult orderResult = request as OrderResult;
                orderResult.Result = response;
                return orderResult;
            });

            Console.ReadKey();
        }
    }
}
