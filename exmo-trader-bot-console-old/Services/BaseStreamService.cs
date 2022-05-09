using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services
{
    public abstract class BaseOutputStreamService<T>
    {
        public IObservable<T> OutputStream
        {
            get => _outputSubject;
            set => value.Subscribe(_outputSubject);
        }

        private readonly ISubject<T> _outputSubject;

        protected BaseOutputStreamService()
        {
            _outputSubject = new Subject<T>();
        }
    }
}
