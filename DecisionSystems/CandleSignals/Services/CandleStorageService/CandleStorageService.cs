using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.CandleStorageService
{
    class CandleStorageService: ICandleStorageService
    {
        public IObservable<Trade[][]> OutputStream => _tradeCandlesSubject;

        public Trade[][] TradeCandles { get; private set; }

        private readonly int _candlesCount;
        private readonly int _candlesMinutes;

        private readonly ISubject<Trade[][]> _tradeCandlesSubject;
        private IObservable<IList<Trade>> _bufferStream;

        public CandleStorageService(Settings settings)
        {
            var candleSystemSettings = settings.Data;
            _candlesCount = candleSystemSettings.CandleCount;
            _candlesMinutes = candleSystemSettings.CandleMinutes;

            TradeCandles = new Trade[_candlesCount][];
            for (int i = 0; i < _candlesCount; i++)
                TradeCandles[i] = new Trade[0];

            _tradeCandlesSubject = new ReplaySubject<Trade[][]>(_candlesMinutes);
        }

        public void Subscribe(IObservable<Trade> inputStream)
        {
            _bufferStream = inputStream.Buffer(TimeSpan.FromMinutes(_candlesMinutes));
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
