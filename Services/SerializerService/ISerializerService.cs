﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.SerializerService
{
    interface ISerializerService<in T> where T : class
    {
        string SerializeRequest(T request);
        IObservable<string> SerializerStream(IObservable<T> requestStream);
    }
}