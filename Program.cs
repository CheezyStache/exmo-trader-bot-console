using System;
using System.Linq;
using System.Reactive.Linq;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.SettingsService;
using exmo_trader_bot_console.Services.WebSocketService;

namespace exmo_trader_bot_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Trader Bot is started!");
            ISettingsService settingsService = new SettingsService();
            IDataWebSocketService webSocketService = new ExmoDataWebSocketService(settingsService);
            IEventParserService eventParserService = new ExmoEventsParserService();
            ITradesParserService tradesParserService = new ExmoTradesParserService();
            IDataStorageService dataStorageService = new DataStorageService();
            DecisionSystems.CandleSignals.Services.DataStorageService.IDataStorageService candlesDataStorage =
                new DecisionSystems.CandleSignals.Services.DataStorageService.DataStorageService(settingsService);

            candlesDataStorage.TradeCandlesStream.Subscribe(t =>
            {
                Console.WriteLine("-------");
                foreach (var candle in t)
                    Console.WriteLine("[ " + string.Join(", ",candle.Select(c => c.Amount.ToString())) + " ]");
            });
            candlesDataStorage.ConnectToTrades(dataStorageService.TradesStream);

            var webSocketStream = webSocketService.ConnectToApi(APIType.Public);
            var responsesWithEventsStream = eventParserService.ParserStream(webSocketStream);

            var updatesStream = responsesWithEventsStream.Where(r => r.Event == ResponseEvent.Update)
                .Select(r => r.Response);

            tradesParserService.ParserStream(updatesStream)
                .Subscribe(dataStorageService.AddTrade);



            Console.ReadKey();
        }
    }
}
