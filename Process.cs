using System;
using System.Reactive.Linq;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.EventRouterService;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.RequestService;
using exmo_trader_bot_console.Services.RESTService;
using exmo_trader_bot_console.Services.WebSocketService;
using Microsoft.Extensions.DependencyInjection;

namespace exmo_trader_bot_console
{
    public class Process
    {
        public static void StartProcess(IServiceProvider provider)
        {
            var dataWebSocketService = provider.GetRequiredService<IDataWebSocketService>();
            var eventParserService = provider.GetRequiredService<IEventParserService>();
            var tradesEventRouterService = provider.GetRequiredService<ITradesEventRouterService>();
            var tradesParserService = provider.GetRequiredService<ITradesParserService>();
            var tradesDataStorageService = provider.GetRequiredService<IDataStorageService<Trade>>();

            var orderRequestService = provider.GetRequiredService<IOrderRequestService>();
            var restService = provider.GetRequiredService<IRestService>();
            var orderResponseParserService = provider.GetRequiredService<IOrderResponseParserService>();


            eventParserService.Subscribe(dataWebSocketService.OutputStream);
            tradesEventRouterService.Subscribe(eventParserService.OutputStream);
            tradesParserService.Subscribe(tradesEventRouterService.OutputStream);
            tradesDataStorageService.Subscribe(tradesParserService.OutputStream);

            restService.Subscribe(orderRequestService.OutputStream);
            orderResponseParserService.Subscribe(restService.OutputStream);

            tradesDataStorageService.OutputStream.Select(o => o.Amount).Subscribe(Console.WriteLine);

            dataWebSocketService.ConnectToApi(APIType.Public);
        }
    }
}
