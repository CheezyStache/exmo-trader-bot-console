using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Internal
{
    public class ResponseWithEvent
    {
        public ResponseWithEvent(string response, ResponseEvent eventProperty)
        {
            Response = response;
            Event = eventProperty;
        }

        public string Response { get; set; }
        public ResponseEvent Event { get; set; }
    }
}
