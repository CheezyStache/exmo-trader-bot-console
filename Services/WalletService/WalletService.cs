using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Models.Wallet;
using exmo_trader_bot_console.Services.SettingsService;

namespace exmo_trader_bot_console.Services.WalletService
{
    class WalletService: IWalletService
    {
        public IObservable<WalletChange> OutputStream => _walletChangeSubject;
        public IDictionary<TradingPair, PairWallet> Wallet { get; }

        private ISubject<WalletChange> _walletChangeSubject;

        public WalletService(ISettingsService<Settings> settingsService)
        {
            _walletChangeSubject = new Subject<WalletChange>();
            Wallet = new Dictionary<TradingPair, PairWallet>();

            var pairSettings = settingsService.GetSettings().Pairs;
            foreach (var pair in pairSettings)
            {
                var pairWallet = new PairWallet(0, pair.CurrencyAmount);
                Wallet.Add(pair.TradingPair, pairWallet);
            }
        }

        public void Subscribe(IObservable<OrderResult> orders)
        {
            orders.Subscribe(ChangeWalletBalance);
        }

        private void ChangeWalletBalance(OrderResult result)
        {
            if (result.Type == TradeType.Buy || result.Type == TradeType.MarketBuyPrice || result.Type == TradeType.MarketBuyQuantity)
            {
                _walletChangeSubject.OnNext(new WalletChange(result.Pair,
                    new PairWallet(result.Quantity, -Wallet[result.Pair].Currency)));

                Wallet[result.Pair].Crypto = result.Quantity;
                Wallet[result.Pair].Currency = 0;

                return;
            }

            if (result.Type == TradeType.Sell || result.Type == TradeType.MarketSellPrice ||
                result.Type == TradeType.MarketSellQuantity)
            {
                _walletChangeSubject.OnNext(new WalletChange(result.Pair,
                    new PairWallet(-Wallet[result.Pair].Crypto, result.Amount)));

                Wallet[result.Pair].Crypto = 0;
                Wallet[result.Pair].Currency = result.Amount;

                return;
            }

            throw new NotImplementedException();
        }
    }
}
