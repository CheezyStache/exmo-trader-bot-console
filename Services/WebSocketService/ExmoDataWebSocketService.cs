using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.SettingsService;

namespace exmo_trader_bot_console.Services.WebSocketService
{
    public class ExmoDataWebSocketService: IDataWebSocketService
    {
        private readonly Settings _settings;
        private readonly IWebSocketService _webSocketService;
        private IWebSocketService _privateWebSocketService;
        private IWebSocketService _publicWebSocketService;
        private readonly ISubject<string> _webSocketSubject;
        private IDisposable _streamConnection;

        public IObservable<string> OutputStream => _webSocketSubject;

        public ExmoDataWebSocketService(ISettingsService<Settings> settings, IWebSocketService webSocketService)
        {
            _settings = settings.GetSettings();
            _webSocketService = webSocketService;
            _webSocketSubject = new Subject<string>();
        }

        public void Subscribe(IObservable<object> inputStream) { }
        
        public void ConnectToApi(APIType type)
        {
            _streamConnection =
                _webSocketService.OutputStream.Subscribe(_webSocketSubject.OnNext, ex => OnWebSocketError(ex, type));

            Task.Run(() => StartConnection(type, _webSocketService));
        }

        private void OnWebSocketError(Exception ex, APIType type)
        {
            if (ex.Data.Contains("CanReconnect") && (bool) ex.Data["CanReconnect"])
            {
                _streamConnection.Dispose();
                ConnectToApi(type);
                return;
            }

            _streamConnection.Dispose();
            _webSocketSubject.OnError(ex);
        }

        private async Task StartConnection(APIType type, IWebSocketService service)
        {
            switch (type)
            {
                case APIType.Orders:
                    _privateWebSocketService = service;
                    await StartOrdersConnection();
                    break;
                case APIType.Trades:
                    _publicWebSocketService = service;
                    await StartTradesConnection();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private async Task StartTradesConnection()
        {
            await _publicWebSocketService.Connect(_settings.Api.ConnectionUrlPublic);
            await SendTrades();
            _publicWebSocketService.StartReceiveStream();
        }

        private async Task StartOrdersConnection()
        {
            await _privateWebSocketService.Connect(_settings.Api.ConnectionUrlPrivate);
            await SendOrders();
            _privateWebSocketService.StartReceiveStream();
        }

        private async Task SendTrades()
        {
            var tradingPairs = _settings.Pairs.Select(p => p.TradingPair)
                .Select(p => $"\"spot/trades:{p.Crypto}_{p.Currency}\"")
                .ToArray();

            var command =
                $"{{\"id\":1,\"method\":\"subscribe\",\"topics\":[{string.Join(",", tradingPairs)}]}}";
            await _publicWebSocketService.Send(command);
        }

        private async Task SendOrders()
        {
            var apiKey = _settings.Api.Key;
            var nonce = DateTime.Now.Ticks;

            var sign = _settings.Api.GetSign(nonce);

            var loginCommand =
                $"{{\"id\":1,\"method\":\"login\",\"api_key\":\"{apiKey}\",\"sign\":\"{sign}\",\"nonce\":{nonce:D}}}";
            await _privateWebSocketService.Send(loginCommand);

            var command =
                $"{{\"id\":2,\"method\":\"subscribe\",\"topics\":[\"spot/user_trades\"]}}";
            await _privateWebSocketService.Send(command);
        }
    }
}
