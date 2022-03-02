using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Decision;

namespace exmo_trader_bot_console.Services.ChartDrawer
{
    interface IChartDrawerService
    {
        void DrawTrendLinesAndSave(Candle[] candles, FlowLine flowLine, string pair, int resolution);
    }
}
