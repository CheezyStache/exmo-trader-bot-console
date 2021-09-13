using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.CandleStorageService
{
    class CandleStorageService
    {
        public IObservable<Trade[][]> OutputStream => _tradeCandlesSubject;

        private Trade[][] _tradeCandles;

        private readonly int _candlesCount;

        private readonly ISubject<Trade[][]> _tradeCandlesSubject;
        private readonly ISubject<Trade> _inputStream;
        private IObservable<IList<Trade>> _bufferStream;

        public CandleStorageService(DataSettings settings)
        {
            var candleSystemSettings = settings;
            _candlesCount = candleSystemSettings.CandleCount;
            var candlesMinutes = candleSystemSettings.CandleMinutes;

            _tradeCandles = new Trade[_candlesCount][];
            for (int i = 0; i < _candlesCount; i++)
                _tradeCandles[i] = new Trade[0];

            _tradeCandlesSubject = new ReplaySubject<Trade[][]>(candlesMinutes);
            _inputStream = new Subject<Trade>();

            _bufferStream = _inputStream.Buffer(TimeSpan.FromMinutes(candlesMinutes));
            _bufferStream.Subscribe(OnCandleForm);
        }

        public void AddTrade(Trade trade)
        {
            _inputStream.OnNext(trade);
        }

        private void OnCandleForm(IList<Trade> trades)
        {
            var tradesArray = trades.ToArray();

            var lastCandleTrade = _tradeCandles.Last().LastOrDefault();
            if(lastCandleTrade != null) 
                tradesArray = trades.Prepend(lastCandleTrade).ToArray();

            _tradeCandles = _tradeCandles.TakeLast(_candlesCount - 1)
                .Append(tradesArray)
                .ToArray();

            _tradeCandlesSubject.OnNext(_tradeCandles);
        }
    }
}
