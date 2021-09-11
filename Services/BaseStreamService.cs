using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services
{
    public abstract class BaseOutputStreamService<T>
    {
        public IObservable<T> OutputStream { get; protected set; }

        protected BaseOutputStreamService()
        {
            OutputStream = Observable.Never<T>();
        }
    }
}
