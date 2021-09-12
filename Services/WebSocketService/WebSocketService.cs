using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Services.LoggerService;

namespace exmo_trader_bot_console.Services.WebSocketService
{
    public class WebSocketService : IWebSocketService
    {
        public IObservable<string> OutputStream => _receiveSubject;
        public void Subscribe(IObservable<object> inputStream) { }

        private readonly ILoggerService _loggerService;
        private readonly Subject<string> _receiveSubject;
        private ClientWebSocket _socket;
        private bool _isReceiving;
        private Task _receiveTask;

        public WebSocketService(ILoggerService loggerService)
        {
            _receiveSubject = new Subject<string>();
            _loggerService = loggerService;
        }

        public Task Connect(string url)
        {
            if (_socket != null)
                throw new Exception("Web socket service has been already connected");

            _socket = new ClientWebSocket();

            return _socket.ConnectAsync(new Uri(url), CancellationToken.None);
        }

        public Task Send(string data)
        {
            return _socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }

        public void StartReceiveStream()
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
                            break;

                        ms.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            var response = await reader.ReadToEndAsync();
                            _receiveSubject.OnNext(response);
                        }
                    }
                } while (_isReceiving);
            }
            catch (Exception e)
            {
                _loggerService.OnInfo("Web socket caught an exception and stopped:", LoggerEvent.Error);
                _loggerService.OnInfo(e.Message, LoggerEvent.Error);
                StopSocket();
                _receiveSubject.OnError(e);
                throw;
            }
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