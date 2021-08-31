using System;
using System.Collections.Generic;
using System.Text;

namespace TraderBot.Models.Exmo
{
    public class ExmoTrades
    {
        public long trade_id { get; set; }
        public string type { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public long date { get; set; }
    }
}
