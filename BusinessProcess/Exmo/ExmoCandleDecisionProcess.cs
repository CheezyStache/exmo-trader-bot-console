using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DataStorageService;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.BusinessProcess.Exmo
{
    class ExmoCandleDecisionProcess: IDecisionProcess
    {
        public IObservable<OrderDecision> DecisionsStream { get; set; }

        public ExmoCandleDecisionProcess(IObservable<Trade> tradeStream, Settings settings)
        {
            IDataStorageService dataStorageService = new DataStorageService(settings, tradeStream);
            IDecisionService decisionService = new CandleSignalsDecisionService(dataStorageService.TradeCandlesStream);
            DecisionsStream = decisionService.DecisionsStream;
        }
    }
}
