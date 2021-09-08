using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.OrderData
{
    class OrderResult: OrderDecision
    {
        public bool Result { get; set; }
        public DateTime Date { get; set; }
    }
}
