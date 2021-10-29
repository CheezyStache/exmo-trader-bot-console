using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Settings
{
    public class SettingsModel
    {
        public PlatformApiSettings Api { get; set; }
        public DataSettings[] Data { get; set; }
    }
}
