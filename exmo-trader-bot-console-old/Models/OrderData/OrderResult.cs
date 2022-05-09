using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Models.OrderData
{
    class OrderResult
    {
        public TradeType Type { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public TradingPair Pair { get; set; }
        public OrderExecType ExecType { get; set; }
        public double CommissionAmount { get; set; }
        public string CommissionCurrency { get; set; }
    }
}
