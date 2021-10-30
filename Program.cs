using System;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.CandleHistory;
using exmo_trader_bot_console.Services.DataStorage;
using exmo_trader_bot_console.Services.Decision;
using exmo_trader_bot_console.Services.EventRouter;
using exmo_trader_bot_console.Services.Logger;
using exmo_trader_bot_console.Services.Mapper;
using exmo_trader_bot_console.Services.OrderMaker;
using exmo_trader_bot_console.Services.OrderResult;
using exmo_trader_bot_console.Services.OrdersJson;
using exmo_trader_bot_console.Services.RESTService;
using exmo_trader_bot_console.Services.Settings;
using exmo_trader_bot_console.Services.TradesJson;
using exmo_trader_bot_console.Services.Wallet;
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

            services.AddSingleton<ISettingsService<SettingsModel>, MainSettingsService>()
                .AddSingleton(mapper)
                .AddSingleton<IMapperService, MapperService>()
                .AddSingleton<IEventRouterService, EventRouterService>()
                .AddSingleton<IOrderMakerService, OrderMakerService>()
                .AddSingleton<IWalletService, WalletService>()
                .AddSingleton<ILoggerService, LoggerService>()
                .AddSingleton<IDecisionService, DecisionService>()
                .AddScoped<ICandleHistoryService, CandleHistoryService>()
                .AddScoped<ITradesJsonService, TradesJsonService>()
                .AddScoped<IOrdersJsonService, OrdersJsonService>()
                .AddScoped<IDataStorageService, DataStorageService>()
                .AddScoped<IOrderResultService, OrderResultService>();
        }
    }
}
