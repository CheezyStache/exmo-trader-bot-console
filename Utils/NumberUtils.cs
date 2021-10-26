using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Utils
{
    public static class NumberUtils
    {
        public static double ParseDouble(string number)
        {
            return Convert.ToDouble(number, CultureInfo.InvariantCulture);
        }
    }
}
