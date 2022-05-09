using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.DataStorageService
{
    public interface IDataStorageService<T>: IStreamService<T, T> where T : class
    {
    }
}
