using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Decision;

namespace exmo_trader_bot_console.Services.Decision
{
    interface IFlowCalcService
    {
        FlowLine CalcFlowLine(Candle[] candles);
        FlowLinePos GetPricePosition(FlowLine flowLine, double price, int candleIndex);
        double FlowPercentRange(FlowLine flowLine, int index);
    }
}
