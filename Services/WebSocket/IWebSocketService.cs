using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.WebSocket
{
    public interface IWebSocketService: IOutputService<string>
    {
        Task Send(string data);
        Task Connect(string url);
        void StartReceiveStream();
    }
}
