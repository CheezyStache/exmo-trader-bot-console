using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.OrderData;

namespace exmo_trader_bot_console.Services.Logger
{
    public interface ILoggerService
    {
        void OnInfo(string info, LoggerEvent loggerEvent);
        void OnException(Exception ex);
        void OnError(ErrorResponse error);

        void OnDecision(OrderDecision decision, bool orderResult);
        void ShowBalance();
        void OnOrder(Models.OrderData.OrderResult orderResult);
    }
}
