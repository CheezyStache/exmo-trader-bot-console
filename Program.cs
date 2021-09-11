using System;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.SettingsService;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.EventRouterService;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.ParserService.Exmo;
using exmo_trader_bot_console.Services.RequestService;
using exmo_trader_bot_console.Services.RESTService;
using exmo_trader_bot_console.Services.SettingsService;
using exmo_trader_bot_console.Services.WebSocketService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace exmo_trader_bot_console
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.WriteLine("Trader Bot is started!");

            using IHost host = CreateHostBuilder(args).Build();

            Process.StartProcess(host.Services);

            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureAppServices);

        static void ConfigureAppServices(HostBuilderContext _, IServiceCollection services)
        {
            services.AddSingleton<ISettingsService<Settings>, MainSettingsService>()
                .AddSingleton<ISettingsService<CandleSignalsSettings>, CandleSettingsService>()
                .AddSingleton<IWebSocketService, WebSocketService>()
                .AddSingleton<IDataWebSocketService, ExmoDataWebSocketService>()
                .AddSingleton<IEventParserService, ExmoEventsParserService>()
                .AddSingleton<ITradesEventRouterService, TradesEventRouterService>()
                .AddSingleton<ITradesParserService, ExmoTradesParserService>()
                .AddSingleton<IDataStorageService<Trade>, TradesDataStorageService>()
                
                .AddSingleton<IOrderRequestService, ExmoOrderRequestService>()
                .AddSingleton<IRestService, ExmoRestService>()
                .AddSingleton<IOrderResponseParserService, ExmoOrderResponseParserService>();
        }
    }
}
