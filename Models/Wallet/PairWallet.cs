using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Wallet
{
    public class PairWallet
    {
        public PairWallet(double crypto, double currency)
        {
            Crypto = crypto;
            Currency = currency;
        }

        public double Crypto { get; set; }
        public double Currency { get; set; }
    }
}
