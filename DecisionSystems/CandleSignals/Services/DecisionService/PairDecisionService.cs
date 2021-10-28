using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services;
using exmo_trader_bot_console.Services.WalletService;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService
{
    class PairDecisionService: IStreamService<bool, OrderDecision>
    {
        public IObservable<OrderDecision> OutputStream => _decisionSubject;

        private readonly IStreamService<Trade[][], OrderDecision>[] _patternCheckServices;
        private readonly IDictionary<int, CandleStorageService.CandleStorageService> _candleStorageServices;

        private readonly ISubject<OrderDecision> _decisionSubject;
        private readonly ISubject<OrderDecision> _allDecisionSubject;

        private readonly int[] _candleMinutes;

        public PairDecisionService(TradingPair pair, CandleSignalsSettings candleSettings, DataSettings settings, IWalletService walletService)
        {
            _decisionSubject = new Subject<OrderDecision>();
            _allDecisionSubject = new Subject<OrderDecision>();

            _candleMinutes = candleSettings.Patterns.Select(p => p.CandleMinutes)
                .Distinct()
                .OrderByDescending(m => m)
                .ToArray();

            _candleStorageServices = new Dictionary<int, CandleStorageService.CandleStorageService>();
            _patternCheckServices = new IStreamService<Trade[][], OrderDecision>[_candleMinutes.Length];

            SetupCollections(pair, candleSettings, settings, walletService);

        }

        public void StoreTrade(Trade trade)
        {
            foreach (var storage in _candleStorageServices)
                storage.Value.AddTrade(trade);
        }

        public void Subscribe(IObservable<bool> inputStream)
        {
            _allDecisionSubject.Throttle(d => inputStream)
                .Subscribe(_decisionSubject);
        }

        private void SetupCollections(TradingPair pair, CandleSignalsSettings candleSettings, DataSettings settings,
            IWalletService walletService)
        {
            for (int i = 0; i < _candleMinutes.Length; i++)
            {
                //var minutes = _candleMinutes[i] == 0 ? settings.CandleMinutes : _candleMinutes[i];
                //var patterns = candleSettings.Patterns.Where(p => p.CandleMinutes == minutes);

                //var patternCheckService = new PatternCheckService(candleSettings, patterns, pair, walletService);
                //_patternCheckServices[i] = patternCheckService;
                //_patternCheckServices[i].OutputStream.Subscribe(_allDecisionSubject.OnNext, _decisionSubject.OnError);

                //var candleStorageService = new CandleStorageService.CandleStorageService(settings.CandleCount, minutes);
                //_candleStorageServices.Add(minutes, candleStorageService);
                //_patternCheckServices[i].Subscribe(candleStorageService.OutputStream);
            }
        }
    }
}
