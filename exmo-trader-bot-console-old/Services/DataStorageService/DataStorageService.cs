using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.DataStorageService
{
    public abstract class DataStorageService<T>: IDataStorageService<T> where T : class
    {
        private readonly ISubject<T> _dataSubject;

        protected DataStorageService()
        {
            _dataSubject = new Subject<T>();
        }

        public IObservable<T> OutputStream => _dataSubject;

        public virtual void Subscribe(IObservable<T> inputStream)
        {
            inputStream.Subscribe(AddData);
        }

        private void AddData(T data)
        {
            _dataSubject.OnNext(data);
        }
    }
}
