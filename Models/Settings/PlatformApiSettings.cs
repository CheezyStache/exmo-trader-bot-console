using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.Settings
{
    public class PlatformApiSettings
    {
        public string ConnectionUrlPublic { get; set; }
        public string ConnectionUrlPrivate { get; set; }
        public string OrderCreatePrivate { get; set; }
        public string CandlesHistoryPublic { get; set; }

        public string Key { get; set; }
        public string SecretKey { get; set; }

        public string GetSign(long nonce)
        {
            using HMACSHA512 hmac = new HMACSHA512(Encoding.UTF8.GetBytes(SecretKey));
            byte[] b = hmac.ComputeHash(Encoding.UTF8.GetBytes(Key + Convert.ToString(nonce)));
            return Convert.ToBase64String(b);
        }

        public string GetSign(string content)
        {
            using HMACSHA512 hmac = new HMACSHA512(Encoding.UTF8.GetBytes(SecretKey));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(content));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
