using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Services.SettingsService
{
    public interface ISettingsService<T> where T: class
    {
        T GetSettings();
    }
}
