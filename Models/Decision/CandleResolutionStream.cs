using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;

namespace exmo_trader_bot_console.Models.Decision
{
    class CandleResolutionStream
    {
        public CandleResolutionStream(int resolution, IObservable<Candle[]> candles)
        {
            Resolution = resolution;
            Candles = candles;
        }

        public int Resolution { get; set; }
        public IObservable<Candle[]> Candles { get; set; }
    }
}
