using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.SettingsService;

namespace exmo_trader_bot_console.Services.DataStorage
{
    public class DataStorageService: IDataStorageService
    {
        private readonly IDictionary<TradingPair, IDictionary<int, IObservable<Candle[]>>> _candleDictionary;
        private readonly Settings _settings;

        public DataStorageService(ISettingsService<Settings> settingsService)
        {
            _candleDictionary = new Dictionary<TradingPair, IDictionary<int, IObservable<Candle[]>>>();
            _settings = settingsService.GetSettings();
        }

        public void Subscribe(IObservable<CandlesSet> inputStream)
        {
            foreach (var dataSettings in _settings.Data)
            {
                var resolutionDictionary = new Dictionary<int, IObservable<Candle[]>>();

                foreach (var chart in dataSettings.Chart)
                {
                    resolutionDictionary.Add(chart.Resolution,
                        inputStream.Where(input => input.Pair.Equals(dataSettings.Pair))
                            .Select(candles => candles.Candles));
                }

                _candleDictionary.Add(dataSettings.Pair, resolutionDictionary);
            }
        }

        public IObservable<Candle[]> GetCandles(TradingPair pair, int resolution)
        {
            return _candleDictionary[pair][resolution];
        }
    }
}
