using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Exmo
{
    public class ExmoSocketResponse
    {
        public long ts { get; set; }

        [JsonPropertyName("event")]
        public string eventProperty { get; set; }

        public long? code { get; set; }
        public string? message { get; set; }
        public string? topic { get; set; }
        public string[]? topics { get; set; }
        public string? error { get; set; }
    }

    public class ExmoSocketResponse<T> : ExmoSocketResponse where T : class
    {
        public T data { get; set; }
    }
}
