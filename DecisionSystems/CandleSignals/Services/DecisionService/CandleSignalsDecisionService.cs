using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DataStorageService;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService
{
    class CandleSignalsDecisionService: IDecisionService
    {
        public IObservable<OrderDecision> DecisionsStream => _decisionsSubject;

        private readonly ISubject<OrderDecision> _decisionsSubject;

        public CandleSignalsDecisionService(IDataStorageService dataStorageService)
        {
            _decisionsSubject = new Subject<OrderDecision>();
            dataStorageService.TradeCandlesStream.Subscribe(MakeDecision);
        }

        private void MakeDecision(Trade[][] candles)
        {

        }
    }
}
