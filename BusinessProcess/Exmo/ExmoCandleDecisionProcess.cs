using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DataStorageService;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.SettingsService;

namespace exmo_trader_bot_console.BusinessProcess.Exmo
{
    class ExmoCandleDecisionProcess: IDecisionProcess
    {
        public IObservable<OrderDecision> DecisionsStream { get; set; }

        public ExmoCandleDecisionProcess(IObservable<Trade> tradeStream, Settings settings)
        {
            var candleSettings = new SettingsService<CandleSignalsSettings>("candlesSettings.json");

            IDataStorageService dataStorageService = new CandleStorageService(settings, tradeStream);
            IDecisionService decisionService = new CandleSignalsDecisionService(dataStorageService.TradeCandlesStream,
                candleSettings.GetSettings(), settings.Pairs[0]);
            DecisionsStream = decisionService.DecisionsStream;
        }
    }
}
