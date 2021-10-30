using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.Settings;
using exmo_trader_bot_console.Services.Wallet;
using RestSharp;

namespace exmo_trader_bot_console.Services.OrderMaker
{
    class OrderMakerService: IOrderMakerService
    {
        public IObservable<bool> OutputStream => _orderResultSubject;

        private readonly ISubject<bool> _orderResultSubject;
        private readonly IWalletService _walletService;
        private readonly SettingsModel _settings;

        public OrderMakerService(IWalletService walletService, ISettingsService<SettingsModel> settingsService)
        {
            _orderResultSubject = new Subject<bool>();
            _walletService = walletService;
            _settings = settingsService.GetSettings();
        }

        public void Subscribe(IObservable<OrderDecision> inputStream)
        {
            inputStream.Subscribe(MakeOrder);
        }

        private void MakeOrder(OrderDecision orderDecision)
        {
            var result = _walletService.CheckBalance(orderDecision.Pair, orderDecision.Type);
            if (!result) return;

            var client = new RestClient(_settings.Api.OrderCreatePrivate);
            client.Timeout = -1;

            var request = SetupRequest(orderDecision);

            IRestResponse response = client.Execute(request);
            var orderCreateResponse = JsonSerializer.Deserialize<ExmoOrderCreateResponse>(response.Content, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

            _orderResultSubject.OnNext(orderCreateResponse?.Result ?? false);
        }

        private RestRequest SetupRequest(OrderDecision orderDecision)
        {
            var balance = _walletService.GetBalance(orderDecision.Pair, orderDecision.Type);

            var type = SerializeTradeType(orderDecision.Type);
            var pair = SerializePair(orderDecision.Pair);

            var request = new RestRequest(Method.POST);

            request.AddParameter("pair", pair);
            request.AddParameter("type", type);
            request.AddParameter("price", "0");
            request.AddParameter("quantity", balance.ToString(CultureInfo.InvariantCulture));

            var nonce = DateTime.Now.Ticks;
            request.AddParameter("nonce", nonce.ToString());

            var content = string.Join("&", request.Parameters.Select(p => p.Name + "=" + p.Value));

            request.AddHeader("Sign", _settings.Api.GetSign(content));
            request.AddHeader("Key", _settings.Api.Key);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            return request;
        }

        private string SerializeTradeType(TradeType type)
        {
            switch (type)
            {
                case TradeType.Buy:
                    return "market_buy_total";
                case TradeType.Sell:
                    return "market_sell";

                default:
                    throw new NotImplementedException();
            }
        }

        private string SerializePair(TradingPair pair)
        {
            return $"{pair.Crypto}_{pair.Currency}";
        }
    }
}
