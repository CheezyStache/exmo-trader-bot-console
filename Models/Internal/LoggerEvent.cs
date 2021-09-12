using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Internal
{
    public enum LoggerEvent
    {
        Buy = ConsoleColor.Green,
        Sell = ConsoleColor.Red,
        Info = ConsoleColor.Cyan,
        Error = ConsoleColor.Yellow,
        Default = ConsoleColor.White
    }
}
