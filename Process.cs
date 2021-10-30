using System;
using System.Reactive.Linq;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.CandleHistory;
using exmo_trader_bot_console.Services.DataStorage;
using exmo_trader_bot_console.Services.Decision;
using exmo_trader_bot_console.Services.EventRouter;
using exmo_trader_bot_console.Services.Logger;
using exmo_trader_bot_console.Services.Mapper;
using exmo_trader_bot_console.Services.OrderMaker;
using exmo_trader_bot_console.Services.OrderResult;
using exmo_trader_bot_console.Services.OrdersJson;
using exmo_trader_bot_console.Services.TradesJson;
using exmo_trader_bot_console.Services.Wallet;
using Microsoft.Extensions.DependencyInjection;
using TraderBot.Models.Exmo;

namespace exmo_trader_bot_console
{
    public class Process
    {
        public static void StartTradesProcess(IServiceProvider services)
        {
            using IServiceScope serviceScope = services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            // -----------------------------------
            // Candles to storage
            // -----------------------------------

            var candleHistoryService = provider.GetRequiredService<ICandleHistoryService>();
            var mapperService = provider.GetRequiredService<IMapperService>();
            var dataStorageService = provider.GetRequiredService<IDataStorageService>();
            var candleStream = mapperService.Map<ExmoCandleSet, CandlesSet>(candleHistoryService.OutputStream);
            dataStorageService.Subscribe(candleStream);

            // -----------------------------------
            // Trades json to object stream
            // -----------------------------------

            var tradesJsonService = provider.GetRequiredService<ITradesJsonService>();
            var eventRouterService = provider.GetRequiredService<IEventRouterService>();
            var tradesResponseStream = mapperService.Deserialize<ResponseWithEvent>(tradesJsonService.OutputStream);
            eventRouterService.Subscribe(tradesResponseStream, "Trades");
            var tradesJsonSuccessStream = eventRouterService.GetRouterStream("Trades", ResponseEvent.Update);
            var exmoTradesStream = mapperService.Deserialize<ExmoTrades[]>(tradesJsonSuccessStream);
            var tradesStream = mapperService.Map<ExmoTrades[], Trade[]>(exmoTradesStream);

            // ----------------------------------
            // Decision and wallet streams
            // ----------------------------------

            var walletService = provider.GetRequiredService<IWalletService>();
            var decisionService = provider.GetRequiredService<IDecisionService>();
            decisionService.Start(tradesStream, walletService.WalletOperationStream);
            var orderMakerService = provider.GetRequiredService<IOrderMakerService>();
            orderMakerService.Subscribe(decisionService.OutputStream);
            var orderResultStream = orderMakerService.OutputStream;

            // ----------------------------------

            var loggerService = provider.GetRequiredService<ILoggerService>();

            Observable.Timer(TimeSpan.FromMinutes(1)).Subscribe(_ => candleHistoryService.GetCandles());
        }

        public static void StartOrdersProcess(IServiceProvider services)
        {
            using IServiceScope serviceScope = services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;

            var eventRouterService = provider.GetRequiredService<IEventRouterService>();
            var mapperService = provider.GetRequiredService<IMapperService>();

            // -----------------------------------
            // Orders json to object stream
            // -----------------------------------

            var ordersJsonService = provider.GetRequiredService<IOrdersJsonService>();
            var ordersResponseStream = mapperService.Deserialize<ResponseWithEvent>(ordersJsonService.OutputStream);
            eventRouterService.Subscribe(ordersResponseStream, "Orders");
            var ordersJsonSuccessStream = eventRouterService.GetRouterStream("Orders", ResponseEvent.Update);
            var exmoOrdersStream = mapperService.Deserialize<ExmoUserTrades>(ordersJsonSuccessStream);
            var ordersStream = mapperService.Map<ExmoUserTrades, OrderResult>(exmoOrdersStream);

            // -----------------------------------
            // Wallet change
            // -----------------------------------

            var orderResultService = provider.GetRequiredService<IOrderResultService>();
            orderResultService.Subscribe(ordersStream);

            var loggerService = provider.GetRequiredService<ILoggerService>();
        }
    }
}
