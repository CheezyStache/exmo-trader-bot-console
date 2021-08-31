using System;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Services.SettingsService;
using exmo_trader_bot_console.Services.WebSocketService;

namespace exmo_trader_bot_console
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            ISettingsService settingsService = new SettingsService();
            IDataWebSocketService webSocketService = new ExmoDataWebSocketService(settingsService);

            webSocketService.ConnectToApi(APIType.Private)
                .Subscribe(Console.WriteLine);

            Console.ReadKey();
        }
    }
}
