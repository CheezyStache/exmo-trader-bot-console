using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace exmo_trader_bot_console.Models.TradingData
{
    public class TradingPair: IEquatable<TradingPair>
    {
        public string Crypto { get; set; }
        public string Currency { get; set; }

        public bool Equals(TradingPair other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Crypto == other.Crypto && Currency == other.Currency;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TradingPair) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Crypto, Currency);
        }
    }
}
