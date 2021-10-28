using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Services.Logger;

namespace exmo_trader_bot_console.Services.WebSocket
{
    public abstract class WebSocketService
    {
        private readonly ILoggerService _loggerService;
        protected readonly Subject<string> ReceiveSubject;

        private ClientWebSocket _socket;

        private bool _isReceiving;
        private Task _receiveTask;

        protected WebSocketService(ILoggerService loggerService)
        {
            ReceiveSubject = new Subject<string>();
            _loggerService = loggerService;
        }

        protected Task Connect(string url)
        {
            if (_socket != null)
                throw new Exception("Web socket service has been already connected");

            _socket = new ClientWebSocket();

            return _socket.ConnectAsync(new Uri(url), CancellationToken.None);
        }

        protected Task Send(string data)
        {
            return _socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }

        protected void StartReceiveStream()
        {
            _isReceiving = true;
            _receiveTask = Task.Run(StartReceiveProcess);
        }

        private async Task StartReceiveProcess()
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);

            try
            {
                do
                {
                    using (var ms = new MemoryStream())
                    {
                        WebSocketReceiveResult result;
                        do
                        {
                            result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                            ms.Write(buffer.Array, buffer.Offset, result.Count);
                        } while (!result.EndOfMessage);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            _loggerService.OnInfo("Websocket closed", LoggerEvent.Info);
                            break;
                        }

                        ms.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            var response = await reader.ReadToEndAsync();
                            ReceiveSubject.OnNext(response);
                        }
                    }
                } while (_isReceiving);
            }
            catch (Exception e)
            {
                HandleException(e);
            }
        }

        private void HandleException(Exception e)
        {
            StopSocket();
            _loggerService.OnException(e);
            ReceiveSubject.OnError(e);
        }

        private void StopSocket()
        {
            _isReceiving = false;
            _receiveTask.Dispose();
            _socket.Dispose();
            _socket = null;
        }
    }
}