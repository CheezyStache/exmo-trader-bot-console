using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService
{
    class PairDecisionService
    {
        public IObservable<OrderDecision> OutputStream => _patternCheckService.OutputStream;

        private readonly IStreamService<Trade[][], OrderDecision> _patternCheckService;
        private readonly CandleStorageService.CandleStorageService _candleStorageService;

        public PairDecisionService(TradingPair pair, CandleSignalsSettings candleSettings, DataSettings settings)
        {
            _patternCheckService = new PatternCheckService(candleSettings, pair);
            _candleStorageService = new CandleStorageService.CandleStorageService(settings);

            _patternCheckService.Subscribe(_candleStorageService.OutputStream);
        }

        public void StoreTrade(Trade trade)
        {
            _candleStorageService.AddTrade(trade);
        }
    }
}
