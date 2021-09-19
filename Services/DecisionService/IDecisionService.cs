using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.DecisionService
{
    interface IDecisionService: IStreamService<Trade, OrderDecision>
    {
        void SubscribeToResultStream(IObservable<bool> resultStream);
    }
}
