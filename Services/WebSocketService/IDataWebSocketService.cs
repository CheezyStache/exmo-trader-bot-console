using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.PlatformAPI;

namespace exmo_trader_bot_console.Services.WebSocketService
{
    public interface IDataWebSocketService
    {
        IObservable<string> ConnectToApi(APIType type);
        void DisconnectFromApi(APIType type);
    }
}
