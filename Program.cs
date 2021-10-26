using System;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.SettingsService;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.CandleHistory;
using exmo_trader_bot_console.Services.DataStorageService;
using exmo_trader_bot_console.Services.DecisionService;
using exmo_trader_bot_console.Services.EventRouter;
using exmo_trader_bot_console.Services.LoggerService;
using exmo_trader_bot_console.Services.Mapper;
using exmo_trader_bot_console.Services.OrdersJson;
using exmo_trader_bot_console.Services.RequestService;
using exmo_trader_bot_console.Services.RESTService;
using exmo_trader_bot_console.Services.SettingsService;
using exmo_trader_bot_console.Services.TradesJson;
using exmo_trader_bot_console.Services.WalletService;
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
                .AddSingleton(mapper)
                .AddSingleton<IMapperService, MapperService>()
                .AddSingleton<IEventRouterService, EventRouterService>()
                .AddSingleton<IDataStorageService<Trade>, TradesDataStorageService>()
                .AddSingleton<IDecisionService, CandleSignalsDecisionService>()
                .AddSingleton<IOrderRequestService, ExmoOrderRequestService>()
                .AddSingleton<IRestService, ExmoRestService>()
                .AddSingleton<IWalletService, WalletService>()
                .AddSingleton<ILoggerService, LoggerService>()
                .AddScoped<ICandleHistoryService, CandleHistoryService>()
                .AddScoped<ITradesJsonService, TradesJsonService>()
                .AddScoped<IOrdersJsonService, OrdersJsonService>();
        }
    }
}
