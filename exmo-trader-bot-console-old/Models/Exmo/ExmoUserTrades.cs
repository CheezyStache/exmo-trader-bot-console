using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Exmo
{
    class ExmoUserTrades
    {
        public long trade_id { get; set; }
        public string type { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string amount { get; set; }
        public long date { get; set; }
        public long order_id { get; set; }
        public string pair { get; set; }
        public string exec_type { get; set; }
        public string commission_amount { get; set; }
        public string commission_currency { get; set; }
        public string commission_percent { get; set; }
    }
}
