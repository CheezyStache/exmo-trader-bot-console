using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.Logger;
using exmo_trader_bot_console.Services.Settings;
using exmo_trader_bot_console.Services.WebSocket;

namespace exmo_trader_bot_console.Services.OrdersJson
{
    class OrdersJsonService : WebSocketService, IOrdersJsonService
    {
        public IObservable<string> OutputStream { get; }

        private readonly Models.Settings.SettingsModel _settings;

        public OrdersJsonService(ISettingsService<Models.Settings.SettingsModel> settings, ILoggerService loggerService) : base(loggerService)
        {
            _settings = settings.GetSettings();

            OutputStream = ReceiveSubject.Catch<string, Exception>((_) =>
            {
                Start();
                return Observable.Empty<string>();
            });
        }

        public void Start()
        {
            Task.Run(StartConnection);
        }

        private async Task StartConnection()
        {
            await Connect(_settings.Api.ConnectionUrlPrivate);

            var apiKey = _settings.Api.Key;
            var nonce = DateTime.Now.Ticks;

            var sign = _settings.Api.GetSign(nonce);

            var loginCommand =
                $"{{\"id\":1,\"method\":\"login\",\"api_key\":\"{apiKey}\",\"sign\":\"{sign}\",\"nonce\":{nonce:D}}}";
            await Send(loginCommand);

            var command =
                $"{{\"id\":2,\"method\":\"subscribe\",\"topics\":[\"spot/user_trades\"]}}";
            await Send(command);

            StartReceiveStream();
        }
    }
}
