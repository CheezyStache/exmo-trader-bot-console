using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Internal
{
    public class ErrorResponse
    {
        public long Code { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
