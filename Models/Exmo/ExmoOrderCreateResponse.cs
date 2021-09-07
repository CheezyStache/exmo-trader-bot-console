using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Exmo
{
    class ExmoOrderCreateResponse
    {
        public bool Result { get; set; }
        public string Error { get; set; }
        public long Order_id { get; set; }
        public long Client_id { get; set; }
    }
}
