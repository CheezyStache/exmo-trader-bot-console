using System;
using System.Reactive.Linq;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.DecisionService;
using exmo_trader_bot_console.Services.EventRouterService;
using exmo_trader_bot_console.Services.LoggerService;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.RequestService;
using exmo_trader_bot_console.Services.RESTService;
using exmo_trader_bot_console.Services.WalletService;
using exmo_trader_bot_console.Services.WebSocketService;
using Microsoft.Extensions.DependencyInjection;

namespace exmo_trader_bot_console
{
    public class Process
    {
        public static void StartTradesProcess(IServiceProvider services)
        {
            using IServiceScope serviceScope = services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var dataWebSocketService = provider.GetRequiredService<IDataWebSocketService>();
            var eventParserService = provider.GetRequiredService<IEventParserService>();
            var updatesEventRouterService = provider.GetRequiredService<IUpdatesEventRouterService>();
            var tradesParserService = provider.GetRequiredService<ITradesParserService>();
            var tradesDataStorageService = provider.GetRequiredService<IDataStorageService<Trade>>();
            var decisionService = provider.GetRequiredService<IDecisionService>();
            var orderRequestService = provider.GetRequiredService<IOrderRequestService>();
            var restService = provider.GetRequiredService<IRestService>();
            var orderResponseParserService = provider.GetRequiredService<IOrderResponseParserService>();
            var loggerService = provider.GetRequiredService<ILoggerService>();

            eventParserService.Subscribe(dataWebSocketService.OutputStream);
            updatesEventRouterService.Subscribe(eventParserService.OutputStream);
            tradesParserService.Subscribe(updatesEventRouterService.OutputStream);
            tradesDataStorageService.Subscribe(tradesParserService.OutputStream);
            decisionService.Subscribe(tradesDataStorageService.OutputStream);
            restService.Subscribe(orderRequestService.OutputStream);
            orderResponseParserService.Subscribe(restService.OutputStream);

            decisionService.OutputStream.Subscribe(loggerService.OnDecision);
            orderResponseParserService.OutputStream.Subscribe(loggerService.OnOrderResult);

            dataWebSocketService.ConnectToApi(APIType.Trades);
        }

        public static void StartOrdersProcess(IServiceProvider services)
        {
            using IServiceScope serviceScope = services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var dataWebSocketService = provider.GetRequiredService<IDataWebSocketService>();
            var eventParserService = provider.GetRequiredService<IEventParserService>();
            var updatesEventRouterService = provider.GetRequiredService<IUpdatesEventRouterService>();
            var orderResultParserService = provider.GetRequiredService<IOrderResultParserService>();
            var walletService = provider.GetRequiredService<IWalletService>();
            var loggerService = provider.GetRequiredService<ILoggerService>();

            eventParserService.Subscribe(dataWebSocketService.OutputStream);
            updatesEventRouterService.Subscribe(eventParserService.OutputStream);
            orderResultParserService.Subscribe(updatesEventRouterService.OutputStream);
            walletService.Subscribe(orderResultParserService.OutputStream);

            walletService.OutputStream.Subscribe(loggerService.OnWalletChange);

            dataWebSocketService.ConnectToApi(APIType.Orders);
        }
    }
}
