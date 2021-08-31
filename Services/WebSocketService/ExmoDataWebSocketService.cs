using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.PlatformAPI;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.SettingsService;

namespace exmo_trader_bot_console.Services.WebSocketService
{
    public class ExmoDataWebSocketService: IDataWebSocketService
    {
        private Settings _settings;
        private IWebSocketService _privateWebSocketService;
        private IWebSocketService _publicWebSocketService;

        public ExmoDataWebSocketService(ISettingsService settingsService)
        {
            _settings = settingsService.GetSettings();
        }

        public IObservable<string> ConnectToApi(APIType type)
        {
            IWebSocketService service = new WebSocketService();

            Task.Run(() => StartConnection(type, service));

            return service.ReceiveStream;
        }

        public void DisconnectFromApi(APIType type)
        {
            switch (type)
            {
                case APIType.Private:
                    _privateWebSocketService.StopReceiveStream();
                    _privateWebSocketService = null;
                    break;
                case APIType.Public:
                    _publicWebSocketService.StopReceiveStream();
                    _publicWebSocketService = null;
                    break;

                default:
                    throw new NotImplementedException();
            }
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
            var secretKey = _settings.Api.SecretKey;
            var nonce = DateTime.Now.Ticks;

            var sign = "";
            using (HMACSHA512 hmac = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(apiKey + Convert.ToString(nonce)));
                sign = Convert.ToBase64String(b);
            }

            var loginCommand =
                $"{{\"id\":1,\"method\":\"login\",\"api_key\":\"{apiKey}\",\"sign\":\"{sign}\",\"nonce\":{nonce:D}}}";
            await _privateWebSocketService.Send(loginCommand);

            var command =
                $"{{\"id\":2,\"method\":\"subscribe\",\"topics\":[]}}";
            await _privateWebSocketService.Send(command);
        }
    }
}
