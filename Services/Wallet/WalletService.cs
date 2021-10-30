using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Models.Wallet;
using exmo_trader_bot_console.Services.Settings;

namespace exmo_trader_bot_console.Services.Wallet
{
    class WalletService: IWalletService
    {
        public IObservable<bool> WalletOperationStream => _walletOperationSubject;
        
        private readonly ISubject<bool> _walletOperationSubject;
        private readonly IDictionary<TradingPair, PairWallet> _wallet;

        public WalletService(ISettingsService<SettingsModel> settingsService)
        {
            _walletOperationSubject = new Subject<bool>();
            _wallet = new Dictionary<TradingPair, PairWallet>();

            var pairSettings = settingsService.GetSettings().Data;
            foreach (var pair in pairSettings)
            {
                var pairWallet = new PairWallet(0, pair.CurrencyAmount);
                _wallet.Add(pair.Pair, pairWallet);
            }
        }

        public double GetBalance(TradingPair pair, TradeType type)
        {
            var balance = _wallet[pair];

            if (type == TradeType.Buy) return balance.Currency;
            if (type == TradeType.Sell) return balance.Crypto;

            return 0;
        }

        public void ChangeBalance(TradingPair pair, TradeType type, double newValue)
        {
            switch (type)
            {
                case TradeType.Buy:
                    _wallet[pair] = new PairWallet(newValue, 0);
                    break;
                case TradeType.Sell:
                    _wallet[pair] = new PairWallet(0, newValue);
                    break;

                default:
                    throw new Exception("Balance trading type is unknown: " + type);
            }

            _walletOperationSubject.OnNext(true);
        }

        public bool CheckBalance(TradingPair pair, TradeType type)
        {
            var balance = _wallet[pair];

            if (type == TradeType.Buy && balance.Currency == 0)
            {
                _walletOperationSubject.OnNext(false);
                return false;
            }

            if (type == TradeType.Sell && balance.Crypto == 0)
            {
                _walletOperationSubject.OnNext(false);
                return false;
            }

            return true;
        }
    }
}
