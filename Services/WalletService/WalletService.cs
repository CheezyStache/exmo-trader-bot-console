using System;
using System.Collections.Generic;
using System.Linq;
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
        public IDictionary<TradingPair, PairWallet> Wallet { get; }

        public WalletService(ISettingsService<Settings> settingsService)
        {
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
                Wallet[result.Pair].Crypto = result.Quantity;
                Wallet[result.Pair].Currency = 0;

                return;
            }

            if (result.Type == TradeType.Sell || result.Type == TradeType.MarketSellPrice ||
                result.Type == TradeType.MarketSellQuantity)
            {
                Wallet[result.Pair].Crypto = 0;
                Wallet[result.Pair].Currency = result.Amount;

                return;
            }

            throw new NotImplementedException();
        }
    }
}
