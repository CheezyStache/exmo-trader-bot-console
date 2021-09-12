using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Models.Wallet;
using exmo_trader_bot_console.Services.WalletService;

namespace exmo_trader_bot_console.Services.LoggerService
{
    class LoggerService: ILoggerService
    {
        private readonly IDictionary<TradingPair, PairWallet> _initialWallet;
        private readonly IWalletService _walletService;

        public LoggerService(IWalletService walletService)
        {
            _walletService = walletService;
            _initialWallet = new Dictionary<TradingPair, PairWallet>();
            foreach (var key in walletService.Wallet.Keys)
            {
                _initialWallet.Add(key, walletService.Wallet[key]);
            }

            ShowBalance();
        }

        public void OnDecision(OrderDecision decision)
        {
            Console.WriteLine($"Decision was made: \"{decision.Description}\"", LoggerEvent.Info);
            if(decision.Type == TradeType.Buy || decision.Type == TradeType.MarketBuyPrice || decision.Type == TradeType.MarketBuyQuantity)
                Console.WriteLine("Buy", LoggerEvent.Buy);
            else if (decision.Type == TradeType.Sell || decision.Type == TradeType.MarketSellPrice ||
                     decision.Type == TradeType.MarketSellQuantity)
                Console.WriteLine("Sell", LoggerEvent.Sell);
            else
                Console.WriteLine("Unknown decision", LoggerEvent.Error);
        }

        public void OnWalletChange(WalletChange walletChange)
        {
            Console.WriteLine("Wallet changed", LoggerEvent.Default);
            Console.WriteLine($"Pair: {walletChange.Pair.Crypto}_{walletChange.Pair.Currency}", LoggerEvent.Info);

            if (walletChange.Changes.Currency < 0)
                Console.WriteLine(
                    $"Bought crypto ({walletChange.Changes.Crypto}{walletChange.Pair.Crypto}) for {walletChange.Changes.Currency * -1}{walletChange.Pair.Currency}",
                    LoggerEvent.Buy);
            else if (walletChange.Changes.Crypto < 0)
                Console.WriteLine(
                    $"Sold crypto ({walletChange.Changes.Crypto * -1}{walletChange.Pair.Crypto}) for {walletChange.Changes.Currency}{walletChange.Pair.Currency}",
                    LoggerEvent.Sell);
            else
                Console.WriteLine("Unknown changes", LoggerEvent.Error);

            ShowBalance();
        }

        public void OnOrderResult(bool result)
        {
            Console.WriteLine("Order result is", LoggerEvent.Default);
            if(result)
                Console.WriteLine("Success", LoggerEvent.Info);
            else
                Console.WriteLine("Error", LoggerEvent.Error);
        }

        public void OnInfo(string info, LoggerEvent loggerEvent)
        {
            Console.WriteLine(info, loggerEvent);
        }

        private void ShowBalance()
        {
            Console.WriteLine();
            Console.WriteLine("------------------", LoggerEvent.Default);

            Console.WriteLine("Current balance:", LoggerEvent.Default);
            foreach (var key in _walletService.Wallet.Keys)
            {
                Console.WriteLine($"Pair: {key.Crypto}_{key.Currency}", LoggerEvent.Info);
                var cryptoDiff = _walletService.Wallet[key].Crypto - _initialWallet[key].Crypto;
                var currencyDiff = _walletService.Wallet[key].Currency - _initialWallet[key].Currency;

                Console.WriteLine($"{key.Crypto}: {_walletService.Wallet[key].Crypto} ({cryptoDiff})", cryptoDiff < 0 ? LoggerEvent.Sell : LoggerEvent.Buy);
                Console.WriteLine($"{key.Currency}: {_walletService.Wallet[key].Currency} ({currencyDiff})", currencyDiff < 0 ? LoggerEvent.Sell : LoggerEvent.Buy);
                Console.WriteLine();
            }

            Console.WriteLine("------------------", LoggerEvent.Default);
            Console.WriteLine();
        }
    }
}
