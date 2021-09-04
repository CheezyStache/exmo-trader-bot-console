using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.SettingsService;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DataStorageService
{
    class DataStorageService: IDataStorageService
    {
        private readonly int _candlesCount;
        private readonly int _candlesMinutes;

        private readonly ISubject<Trade[][]> _tradeCandlesSubject;
        private IObservable<IList<Trade>> _bufferStream;

        public DataStorageService(ISettingsService settingsService)
        {
            var settings = settingsService.GetSettings().CandleSystem;
            _candlesCount = settings.CandleCount;
            _candlesMinutes = settings.CandleMinutes;

            TradeCandles = new Trade[_candlesCount][];
            for (int i = 0; i < _candlesCount; i++)
                TradeCandles[i] = new Trade[0];

            _tradeCandlesSubject = new ReplaySubject<Trade[][]>(_candlesMinutes);
        }

        public Trade[][] TradeCandles { get; private set; }
        public IObservable<Trade[][]> TradeCandlesStream => _tradeCandlesSubject;

        public void ConnectToTrades(IObservable<Trade> tradesStream)
        {
            if (_bufferStream != null)
                throw new Exception("Candle signals data storage is already connected to trades stream");

            _bufferStream = tradesStream.Buffer(TimeSpan.FromMinutes(_candlesMinutes));
            _bufferStream.Subscribe(OnCandleForm);
        }

        private void OnCandleForm(IList<Trade> trades)
        {
            var tradesArray = trades.ToArray();

            TradeCandles = TradeCandles.TakeLast(_candlesCount - 1)
                .Append(tradesArray)
                .ToArray();

            _tradeCandlesSubject.OnNext(TradeCandles);
        }
    }
}
