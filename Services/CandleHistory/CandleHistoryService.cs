using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
    class CandleHistoryService : ICandleHistoryService
    {
        private readonly Settings _settings;
        private readonly ISubject<CandlesSet> _candleSubject;

        public IObservable<CandlesSet> OutputStream => _candleSubject;

        public CandleHistoryService(ISettingsService<Settings> settingsService)
        {
            _settings = settingsService.GetSettings();
        }

        public void GetCandles()
        {
            SetupOutputStream(_settings.Data);
        }

        public void GetCandles(IEnumerable<DataSettings> settings)
        {
            SetupOutputStream(settings);
        }

        private void SetupOutputStream(IEnumerable<DataSettings> settings)
        {
            foreach (var setting in settings)
            {
                foreach (var chart in setting.Chart)
                {
                    var candleSet = GetCandleSet(setting.Pair, chart.Resolution, chart.Resolution * chart.StartCandles);
                    _candleSubject.OnNext(candleSet);
                }
            }
        }

        private CandlesSet GetCandleSet(TradingPair pair, int resolution, int minutesRange)
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

            var candlesSet = JsonSerializer.Deserialize<CandlesSet>(response.Content, new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            candlesSet.Resolution = resolution;
            candlesSet.Pair = pair;

            return candlesSet;
        }
    }
}