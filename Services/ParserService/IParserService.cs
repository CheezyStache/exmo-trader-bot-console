﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.ParserService
{
    public interface IParserService<out T> where T: class
    {
        IEnumerable<T> ParseResponse(string response);
        IObservable<T> ParserStream(IObservable<string> responseStream);
    }
}
