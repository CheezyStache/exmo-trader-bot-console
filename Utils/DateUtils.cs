using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Utils
{
    public static class DateUtils
    {
        public static DateTime GetDate(long date)
        {
            date *= 1000;
            return new DateTime(1970, 01, 01).AddMilliseconds(date);
        }

        public static long GetDate(DateTime date)
        {
            var timespan = date - new DateTime(1970, 01, 01);
            return timespan.Milliseconds / 1000;
        }
    }
}
