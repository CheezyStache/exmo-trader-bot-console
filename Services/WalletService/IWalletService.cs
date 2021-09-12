using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Models.Wallet;

namespace exmo_trader_bot_console.Services.WalletService
{
    interface IWalletService
    {
        IDictionary<TradingPair, PairWallet> Wallet { get; }
        void Subscribe(IObservable<OrderResult> orders);
    }
}
