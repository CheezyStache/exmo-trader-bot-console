using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DataStorageService
{
    class DataStorageService: IDataStorageService
    {
        public Trade[][] TradeCandles { get; private set; }
        public IObservable<Trade[][]> TradeCandlesStream => _tradeCandlesSubject;

        private readonly int _candlesCount;
        private readonly int _candlesMinutes;

        private readonly ISubject<Trade[][]> _tradeCandlesSubject;
        private IObservable<IList<Trade>> _bufferStream;

        public DataStorageService(Settings settings, IObservable<Trade> tradesStream)
        {
            var candleSystemSettings = settings.Data;
            _candlesCount = candleSystemSettings.CandleCount;
            _candlesMinutes = candleSystemSettings.CandleMinutes;

            TradeCandles = new Trade[_candlesCount][];
            for (int i = 0; i < _candlesCount; i++)
                TradeCandles[i] = new Trade[0];

            _tradeCandlesSubject = new ReplaySubject<Trade[][]>(_candlesMinutes);

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
