﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.WebSocketService
{
    public interface IWebSocketService
    {
        IObservable<string> ReceiveStream { get; }
        Task Send(string data);
        Task Connect(string url);
        void StartReceiveStream();
        void StopReceiveStream();
    }
}
