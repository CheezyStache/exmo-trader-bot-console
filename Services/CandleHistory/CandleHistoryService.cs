using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.SettingsService;
using exmo_trader_bot_console.Utils;
using RestSharp;

namespace exmo_trader_bot_console.Services.CandleHistory
{
    class CandleHistoryService: ICandleHistoryService
    {
        private readonly Settings _settings;

        public IObservable<CandlesSet> OutputStream { get; }

        public CandleHistoryService(ISettingsService<Settings> settingsService)
        {
            _settings = settingsService.GetSettings();
            OutputStream = SetupOutputStream();
        }

        private IObservable<CandlesSet> SetupOutputStream()
        {
            return _settings.Pairs.ToObservable()
                .SelectMany(pair => _settings.Data.Select(d => new {Data = d, Pair = pair}))
                .Select(set => GetCandles(set.Pair.TradingPair, set.Data.Resolution,
                    set.Data.Resolution * set.Data.StartCandles));
        }

        private CandlesSet GetCandles(TradingPair pair, int resolution, int minutesRange)
        {
            var symbol = $"{pair.Crypto}_{pair.Currency}";
            var from = DateUtils.GetDate(DateTime.Now - TimeSpan.FromMinutes(minutesRange));
            var to = DateUtils.GetDate(DateTime.Now);

            var url =
                $"{_settings.Api.CandlesHistoryPublic}?symbol={symbol}&resolution={resolution}&from={from}&to={to}";

            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var candlesSet = JsonSerializer.Deserialize<CandlesSet>(response.Content);
            candlesSet.Resolution = resolution;
            candlesSet.Pair = pair;

            return candlesSet;
        }
    }
}
