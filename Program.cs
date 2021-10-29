using System;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.CandleHistory;
using exmo_trader_bot_console.Services.DataStorage;
using exmo_trader_bot_console.Services.Decision;
using exmo_trader_bot_console.Services.EventRouter;
using exmo_trader_bot_console.Services.Logger;
using exmo_trader_bot_console.Services.Mapper;
using exmo_trader_bot_console.Services.OrdersJson;
using exmo_trader_bot_console.Services.RequestService;
using exmo_trader_bot_console.Services.RESTService;
using exmo_trader_bot_console.Services.Settings;
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

            services.AddSingleton<ISettingsService<SettingsModel>, MainSettingsService>()
                .AddSingleton(mapper)
                .AddSingleton<IMapperService, MapperService>()
                .AddSingleton<IEventRouterService, EventRouterService>()
                .AddSingleton<IDataStorageService, DataStorageService>()
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
