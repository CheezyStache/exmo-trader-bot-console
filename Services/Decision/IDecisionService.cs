using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.Decision
{
    interface IDecisionService: IOutputService<OrderDecision>
    {
        void Start(IObservable<Trade[]> tradesStream, IObservable<bool> decisionSentStream);
    }
}
