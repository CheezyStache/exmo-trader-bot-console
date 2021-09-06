using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.OrderData;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService
{
    interface IDecisionService
    {
        IObservable<OrderDecision> DecisionsStream { get; }
    }
}
