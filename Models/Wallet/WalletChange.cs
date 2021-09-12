using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Models.Wallet
{
    public class WalletChange
    {
        public WalletChange(TradingPair pair, PairWallet changes)
        {
            Pair = pair;
            Changes = changes;
        }

        public TradingPair Pair { get; set; }
        public PairWallet Changes { get; set; }
    }
}
