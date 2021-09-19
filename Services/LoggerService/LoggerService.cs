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
            PrintDate();
            WriteLine($"Decision was made: \"{decision.Description}\"", LoggerEvent.Info);
            if(decision.Type == TradeType.Buy || decision.Type == TradeType.MarketBuyPrice || decision.Type == TradeType.MarketBuyQuantity)
                WriteLine("Buy", LoggerEvent.Buy);
            else if (decision.Type == TradeType.Sell || decision.Type == TradeType.MarketSellPrice ||
                     decision.Type == TradeType.MarketSellQuantity)
                WriteLine("Sell", LoggerEvent.Sell);
            else
                WriteLine("Unknown decision", LoggerEvent.Error);
        }

        public void OnWalletChange(WalletChange walletChange)
        {
            PrintDate();
            WriteLine("Wallet changed");
            WriteLine($"Pair: {walletChange.Pair.Crypto}_{walletChange.Pair.Currency}", LoggerEvent.Info);

            if (walletChange.Changes.Currency < 0)
                WriteLine(
                    $"Bought crypto ({walletChange.Changes.Crypto}{walletChange.Pair.Crypto}) for {walletChange.Changes.Currency * -1}{walletChange.Pair.Currency}",
                    LoggerEvent.Buy);
            else if (walletChange.Changes.Crypto < 0)
                WriteLine(
                    $"Sold crypto ({walletChange.Changes.Crypto * -1}{walletChange.Pair.Crypto}) for {walletChange.Changes.Currency}{walletChange.Pair.Currency}",
                    LoggerEvent.Sell);
            else
                WriteLine("Unknown changes", LoggerEvent.Error);

            ShowBalance();
        }

        public void OnOrderResult(bool result)
        {
            PrintDate();
            WriteLine("Order result is");
            if(result)
                WriteLine("Success", LoggerEvent.Info);
            else
                WriteLine("Error", LoggerEvent.Error);
        }

        public void OnInfo(string info, LoggerEvent loggerEvent)
        {
            PrintDate();
            WriteLine(info, loggerEvent);
        }

        public void OnException(Exception ex)
        {
            PrintDate();
            WriteLine("Exception occurred:", LoggerEvent.Error);
            WriteLine(ex.Source, LoggerEvent.Error);
            WriteLine(ex.Message, LoggerEvent.Error);
        }

        public void OnError(ErrorResponse error)
        {
            PrintDate();
            WriteLine("Error returned from server:", LoggerEvent.Error);
            WriteLine(error.Message, LoggerEvent.Error);
        }

        private void ShowBalance()
        {
            WriteLine();
            WriteLine("------------------");

            WriteLine("Current balance:");
            foreach (var key in _walletService.Wallet.Keys)
            {
                WriteLine($"Pair: {key.Crypto}_{key.Currency}", LoggerEvent.Info);
                var cryptoDiff = _walletService.Wallet[key].Crypto - _initialWallet[key].Crypto;
                var currencyDiff = _walletService.Wallet[key].Currency - _initialWallet[key].Currency;

                WriteLine($"{key.Crypto}: {_walletService.Wallet[key].Crypto} ({cryptoDiff})", cryptoDiff < 0 ? LoggerEvent.Sell : LoggerEvent.Buy);
                WriteLine($"{key.Currency}: {_walletService.Wallet[key].Currency} ({currencyDiff})", currencyDiff < 0 ? LoggerEvent.Sell : LoggerEvent.Buy);
                WriteLine();
            }

            WriteLine("------------------");
            WriteLine();
        }

        private void WriteLine(string message = "", LoggerEvent loggerEvent = LoggerEvent.Default)
        {
            Console.ForegroundColor = (ConsoleColor)loggerEvent;
            Console.WriteLine(message);
        }

        private void PrintDate()
        {
            WriteLine();
            WriteLine(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));
        }
    }
}
