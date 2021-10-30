using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.Wallet
{
    interface IWalletService
    {
        double GetBalance(TradingPair pair, TradeType type);
        void ChangeBalance(TradingPair pair, TradeType type, double newValue);
        bool CheckBalance(TradingPair pair, TradeType type);
        IObservable<bool> WalletOperationStream { get; }
    }
}
