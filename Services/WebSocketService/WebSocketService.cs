using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.WebSocketService
{
    public class WebSocketService : IWebSocketService, IDisposable
    {
        public IObservable<string> ReceiveStream => _receiveSubject;

        private readonly Subject<string> _receiveSubject;
        private ClientWebSocket _socket;
        private bool _isReceiving;
        private Task _receiveTask;

        public WebSocketService()
        {
            _receiveSubject = new Subject<string>();
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

        public void StopReceiveStream()
        {
            Dispose();
            _receiveSubject.OnCompleted();
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
                Dispose();
                _receiveSubject.OnError(e);
                throw;
            }
        }

        public void Dispose()
        {
            _isReceiving = false;
            _receiveTask.Dispose();
            _socket.Dispose();
            _socket = null;
        }
    }
}