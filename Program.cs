using System;
using System.Linq;
using System.Reactive.Linq;
using exmo_trader_bot_console.BusinessProcess;
using exmo_trader_bot_console.BusinessProcess.Exmo;
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

            IDecisionProcess decisionProcess =
                new ExmoCandleDecisionProcess(dataStorageFillerProcess.TradesStream, settings);

            IOrderMakerProcess orderMakerProcess = new ExmoOrderMakerProcess(decisionProcess.DecisionsStream, settings);
            orderMakerProcess.OrderResultStream.Subscribe(Console.WriteLine);

            Console.ReadKey();
        }
    }
}
