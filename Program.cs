using System;
using System.Threading.Tasks;
using AutoMapper;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.SettingsService;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.DecisionService;
using exmo_trader_bot_console.Services.EventRouterService;
using exmo_trader_bot_console.Services.LoggerService;
using exmo_trader_bot_console.Services.Mapper;
using exmo_trader_bot_console.Services.ParserService;
using exmo_trader_bot_console.Services.ParserService.Exmo;
using exmo_trader_bot_console.Services.RequestService;
using exmo_trader_bot_console.Services.RESTService;
using exmo_trader_bot_console.Services.SettingsService;
using exmo_trader_bot_console.Services.WalletService;
using exmo_trader_bot_console.Services.WebSocket;
using exmo_trader_bot_console.Services.WebSocketService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace exmo_trader_bot_console
{
    class Program
    {
        static Task Main(string[] args)
        {
            Console.WriteLine("Trader Bot is started!");

            using IHost host = CreateHostBuilder(args).Build();

            Process.StartTradesProcess(host.Services);
            Process.StartOrdersProcess(host.Services);

            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(ConfigureAppServices)
                .ConfigureLogging(loggingBuilder => loggingBuilder.ClearProviders());

        static void ConfigureAppServices(HostBuilderContext _, IServiceCollection services)
        {
            var mapperConfiguration = new MapperConfigurationService();
            var mapper = mapperConfiguration.CreateMapper();

            services.AddSingleton<ISettingsService<Settings>, MainSettingsService>()
                .AddSingleton<ISettingsService<CandleSignalsSettings>, CandleSettingsService>()
                .AddSingleton<ITradesParserService, ExmoTradesParserService>()
                .AddSingleton<IDataStorageService<Trade>, TradesDataStorageService>()
                .AddSingleton<IDecisionService, CandleSignalsDecisionService>()
                .AddSingleton<IOrderRequestService, ExmoOrderRequestService>()
                .AddSingleton<IRestService, ExmoRestService>()
                .AddSingleton<IOrderResponseParserService, ExmoOrderResponseParserService>()
                .AddSingleton<IOrderResultParserService, ExmoOrderResultParserService>()
                .AddSingleton<IWalletService, WalletService>()
                .AddSingleton<ILoggerService, LoggerService>()
                .AddSingleton(mapper)
                .AddScoped<IWebSocketService, WebSocketService>()
                .AddScoped<IDataWebSocketService, ExmoDataWebSocketService>()
                .AddScoped<IEventParserService, ExmoEventsParserService>()
                .AddScoped<IUpdatesEventRouterService, UpdatesEventRouterService>()
                .AddScoped<IErrorsEventRouterService, ErrorsEventRouterService>()
                .AddScoped<IErrorParserService, ExmoErrorParserService>();
        }
    }
}
