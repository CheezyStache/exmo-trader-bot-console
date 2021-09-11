using System;
using System.Reactive.Linq;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.EventRouterService;
using exmo_trader_bot_console.Services.ParserService;
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

            eventParserService.Subscribe(dataWebSocketService.OutputStream);
            tradesEventRouterService.Subscribe(eventParserService.OutputStream);
            tradesParserService.Subscribe(tradesEventRouterService.OutputStream);
            tradesDataStorageService.Subscribe(tradesParserService.OutputStream);

            tradesDataStorageService.OutputStream.Select(o => o.Amount).Subscribe(Console.WriteLine);

            dataWebSocketService.ConnectToApi(APIType.Public);
        }
    }
}
