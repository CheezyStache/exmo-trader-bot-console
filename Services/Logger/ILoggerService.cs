using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Wallet;

namespace exmo_trader_bot_console.Services.Logger
{
    public interface ILoggerService
    {
        void OnDecision(OrderDecision decision);
        void OnWalletChange(WalletChange walletChange);
        void OnOrderResult(bool result);
        void OnInfo(string info, LoggerEvent loggerEvent);
        void OnException(Exception ex);
        void OnError(ErrorResponse error);
    }
}
