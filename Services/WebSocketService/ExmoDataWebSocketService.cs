using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Models.Settings;

namespace exmo_trader_bot_console.Services.WebSocketService
{
    public class ExmoDataWebSocketService: BaseOutputStreamService<string>, IDataWebSocketService
    {
        private Settings _settings;
        private IWebSocketService _privateWebSocketService;
        private IWebSocketService _publicWebSocketService;

        public ExmoDataWebSocketService(Settings settings)
        {
            _settings = settings;
        }

        public void Subscribe(IObservable<object> inputStream) { }

        public void ConnectToApi(APIType type)
        {
            IWebSocketService service = new WebSocketService();
            OutputStream = service.OutputStream;

            Task.Run(() => StartConnection(type, service));
        }

        private async Task StartConnection(APIType type, IWebSocketService service)
        {
            switch (type)
            {
                case APIType.Private:
                    _privateWebSocketService = service;
                    await StartPrivateConnection();
                    break;
                case APIType.Public:
                    _publicWebSocketService = service;
                    await StartPublicConnection();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        private async Task StartPublicConnection()
        {
            await _publicWebSocketService.Connect(_settings.Api.ConnectionUrlPublic);
            await SendPublic();
            _publicWebSocketService.StartReceiveStream();
        }

        private async Task StartPrivateConnection()
        {
            await _privateWebSocketService.Connect(_settings.Api.ConnectionUrlPrivate);
            await SendPrivate();
            _privateWebSocketService.StartReceiveStream();
        }

        private async Task SendPublic()
        {
            var tradingPairs = _settings.Pairs.Select(p => p.TradingPair)
                .Select(p => $"\"spot/trades:{p.Crypto}_{p.Currency}\"")
                .ToArray();

            var command =
                $"{{\"id\":1,\"method\":\"subscribe\",\"topics\":[{string.Join(",", tradingPairs)}]}}";
            await _publicWebSocketService.Send(command);
        }

        private async Task SendPrivate()
        {
            var apiKey = _settings.Api.Key;
            var nonce = DateTime.Now.Ticks;

            var sign = _settings.Api.GetSign(nonce);

            var loginCommand =
                $"{{\"id\":1,\"method\":\"login\",\"api_key\":\"{apiKey}\",\"sign\":\"{sign}\",\"nonce\":{nonce:D}}}";
            await _privateWebSocketService.Send(loginCommand);

            var command =
                $"{{\"id\":2,\"method\":\"subscribe\",\"topics\":[]}}";
            await _privateWebSocketService.Send(command);
        }
    }
}
