using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DecisionService;
using exmo_trader_bot_console.Services.SettingsService;
using exmo_trader_bot_console.Services.WalletService;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService
{
    class CandleSignalsDecisionService: IDecisionService
    {
        public IObservable<OrderDecision> OutputStream => _decisionsSubject;
        
        private readonly IDictionary<TradingPair, PairDecisionService> _pairServices;
        private readonly ISubject<OrderDecision> _decisionsSubject;
        private readonly ISubject<bool> _resultsSubject;
        private readonly CandleSignalsSettings _candleSettings;
        private readonly Settings _settings;
        private readonly IWalletService _walletService;

        public CandleSignalsDecisionService(ISettingsService<Settings> settingsService,
            ISettingsService<CandleSignalsSettings> candleSettingsService, IWalletService walletService)
        {
            _settings = settingsService.GetSettings();
            _candleSettings = candleSettingsService.GetSettings();
            _walletService = walletService;

            _resultsSubject = new Subject<bool>();
            _decisionsSubject = new Subject<OrderDecision>();
            _pairServices = new Dictionary<TradingPair, PairDecisionService>();
        }

        public void Subscribe(IObservable<Trade> inputStream)
        {
            inputStream.Subscribe(SetupPairDecisionService);
        }

        public void SubscribeToResultStream(IObservable<bool> resultStream)
        {
            resultStream.Subscribe(_resultsSubject);
        }

        private void SetupPairDecisionService(Trade trade)
        {
            var pair = trade.Pair;
            if (_pairServices.ContainsKey(pair))
            {
                _pairServices[pair].StoreTrade(trade);
                return;
            }

            var pairService = new PairDecisionService(pair, _candleSettings, _settings.Data, _walletService);
            pairService.OutputStream.Subscribe(_decisionsSubject);
            _pairServices.Add(pair, pairService);
            _pairServices[pair].StoreTrade(trade);
        }
    }
}
