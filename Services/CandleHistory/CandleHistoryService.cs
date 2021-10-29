using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Exmo;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.Settings;
using exmo_trader_bot_console.Utils;
using RestSharp;

namespace exmo_trader_bot_console.Services.CandleHistory
{
    class CandleHistoryService : ICandleHistoryService
    {
        private readonly Models.Settings.SettingsModel _settings;
        private readonly ISubject<ExmoCandleSet> _candleSubject;
        private readonly DateTime _botStart;

        public IObservable<ExmoCandleSet> OutputStream => _candleSubject;

        public CandleHistoryService(ISettingsService<Models.Settings.SettingsModel> settingsService)
        {
            _settings = settingsService.GetSettings();
            _candleSubject = new Subject<ExmoCandleSet>();
            _botStart = DateTime.UtcNow;
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

                    if(candleSet.Candles != null)
                        _candleSubject.OnNext(candleSet);
                }
            }
        }

        private ExmoCandleSet GetCandleSet(TradingPair pair, int resolution, int minutesRange)
        {
            var symbol = $"{pair.Crypto}_{pair.Currency}";
            var from = DateUtils.GetDate(_botStart - TimeSpan.FromMinutes(minutesRange));
            var to = DateUtils.GetDate(DateTime.UtcNow);

            var url =
                $"{_settings.Api.CandlesHistoryPublic}?symbol={symbol}&resolution={resolution}&from={from}&to={to}";

            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            var candlesSet = JsonSerializer.Deserialize<ExmoCandleSet>(response.Content, new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            candlesSet.Resolution = resolution;
            candlesSet.Pair = pair;

            return candlesSet;
        }
    }
}