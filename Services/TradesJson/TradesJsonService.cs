using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.LoggerService;
using exmo_trader_bot_console.Services.SettingsService;
using exmo_trader_bot_console.Services.WebSocket;

namespace exmo_trader_bot_console.Services.TradesJson
{
    class TradesJsonService: WebSocketService, ITradesJsonService
    {
        public IObservable<string> OutputStream { get; }

        private readonly Settings _settings;

        public TradesJsonService(ISettingsService<Settings> settings, ILoggerService loggerService): base(loggerService)
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
            await Connect(_settings.Api.ConnectionUrlPublic);

            var tradingPairs = _settings.Pairs.Select(p => p.TradingPair)
                .Select(p => $"\"spot/trades:{p.Crypto}_{p.Currency}\"")
                .ToArray();

            var command =
                $"{{\"id\":1,\"method\":\"subscribe\",\"topics\":[{string.Join(",", tradingPairs)}]}}";
            await Send(command);

            StartReceiveStream();
        }
    }
}
