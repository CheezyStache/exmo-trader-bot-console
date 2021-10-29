using System;
using System.Reactive.Linq;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.CandleHistory;
using exmo_trader_bot_console.Services.DataStorage;
using exmo_trader_bot_console.Services.EventRouter;
using exmo_trader_bot_console.Services.Logger;
using exmo_trader_bot_console.Services.Mapper;
using exmo_trader_bot_console.Services.TradesJson;
using Microsoft.Extensions.DependencyInjection;

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
            var tradesStream = mapperService.Deserialize<Trade[]>(tradesJsonSuccessStream);

            // ----------------------------------

            var loggerService = provider.GetRequiredService<ILoggerService>();
            dataStorageService.GetCandles(new TradingPair {Crypto = "XRP", Currency = "USD"}, 1).Subscribe(candles =>
            {
                foreach (var candle in candles)
                {
                    loggerService.OnInfo($"{candle.Open} -> {candle.Close}", LoggerEvent.Info);
                }
            });

            Observable.Timer(TimeSpan.FromMinutes(1)).Subscribe(time => candleHistoryService.GetCandles());
        }

        public static void StartOrdersProcess(IServiceProvider services)
        {
            //using IServiceScope serviceScope = services.CreateScope();
            //IServiceProvider provider = serviceScope.ServiceProvider;

            //var dataWebSocketService = provider.GetRequiredService<IDataWebSocketService>();
            //var eventParserService = provider.GetRequiredService<IEventParserService>();
            //var updatesEventRouterService = provider.GetRequiredService<IUpdatesEventRouterService>();
            //var orderResultParserService = provider.GetRequiredService<IOrderResultParserService>();
            //var walletService = provider.GetRequiredService<IWalletService>();
            //var loggerService = provider.GetRequiredService<ILoggerService>();

            //var errorsEventRouterService = provider.GetRequiredService<IErrorsEventRouterService>();
            //var errorParserService = provider.GetRequiredService<IErrorParserService>();

            //eventParserService.Subscribe(dataWebSocketService.OutputStream);
            //updatesEventRouterService.Subscribe(eventParserService.OutputStream);
            //orderResultParserService.Subscribe(updatesEventRouterService.OutputStream);
            //walletService.Subscribe(orderResultParserService.OutputStream);

            //walletService.OutputStream.Subscribe(loggerService.OnWalletChange, loggerService.OnException);

            //errorsEventRouterService.Subscribe(eventParserService.OutputStream);
            //errorParserService.Subscribe(errorsEventRouterService.OutputStream);
            //errorParserService.OutputStream.Subscribe(loggerService.OnError, loggerService.OnException);

            //dataWebSocketService.ConnectToApi(APIType.Orders);
        }
    }
}
