using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Settings
{
    public class PlatformApiSettings
    {
        public string ConnectionUrlPublic { get; set; }
        public string ConnectionUrlPrivate { get; set; }

        public string Key { get; set; }
        public string SecretKey { get; set; }
    }
}
