using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Internal;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Models.Wallet;
using exmo_trader_bot_console.Services.Settings;
using exmo_trader_bot_console.Services.Wallet;

namespace exmo_trader_bot_console.Services.Logger
{
    class LoggerService: ILoggerService
    {
        private readonly IDictionary<TradingPair, PairWallet> _initialWallet;
        private readonly IWalletService _walletService;
        private readonly SettingsModel _settingsModel;

        public LoggerService(IWalletService walletService, ISettingsService<SettingsModel> settingsService)
        {
            _walletService = walletService;
            _settingsModel = settingsService.GetSettings();
            _initialWallet = new Dictionary<TradingPair, PairWallet>();
            foreach (var key in _settingsModel.Data)
            {
                var pairWallet = new PairWallet(0, key.CurrencyAmount);
                _initialWallet.Add(key.Pair, pairWallet);
            }

            ShowBalance();
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

        public void OnDecision(OrderDecision decision, bool orderResult)
        {
            PrintDate();

            if (!orderResult)
            {
                WriteLine("Decision was refused on EXMO", LoggerEvent.Error);
                return;
            }

            switch (decision.Type)
            {
                case TradeType.Buy:
                    WriteLine($"New decision: BUY {decision.Pair.Crypto}_{decision.Pair.Currency}", LoggerEvent.Buy);
                    break;

                case TradeType.Sell:
                    WriteLine($"New decision: SELL {decision.Pair.Crypto}_{decision.Pair.Currency}", LoggerEvent.Sell);
                    break;
            }
        }

        public void ShowBalance()
        {
            WriteLine();
            WriteLine("------------------");

            WriteLine("Current balance:");
            foreach (var key in _settingsModel.Data)
            {
                var cryptoBalance = _walletService.GetBalance(key.Pair, TradeType.Sell);
                var currencyBalance = _walletService.GetBalance(key.Pair, TradeType.Buy);

                WriteLine($"Pair: {key.Pair.Crypto}_{key.Pair.Currency}", LoggerEvent.Info);
                var cryptoDiff = cryptoBalance - _initialWallet[key.Pair].Crypto;
                var currencyDiff = currencyBalance - _initialWallet[key.Pair].Currency;

                WriteLine($"{key.Pair.Crypto}: {cryptoBalance} ({cryptoDiff})", cryptoDiff < 0 ? LoggerEvent.Sell : LoggerEvent.Buy);
                WriteLine($"{key.Pair.Currency}: {currencyBalance} ({currencyDiff})", currencyDiff < 0 ? LoggerEvent.Sell : LoggerEvent.Buy);
                WriteLine();
            }

            WriteLine("------------------");
            WriteLine();
        }

        public void OnOrder(Models.OrderData.OrderResult orderResult)
        {
            PrintDate();
            WriteLine("Order was executed");

            switch (orderResult.Type)
            {
                case TradeType.Buy:
                    WriteLine($"BUY {orderResult.Quantity} {orderResult.Pair.Crypto} for {orderResult.Amount} {orderResult.Pair.Currency}", LoggerEvent.Buy);
                    break;

                case TradeType.Sell:
                    WriteLine($"SELL {orderResult.Quantity} {orderResult.Pair.Crypto} for {orderResult.Amount} {orderResult.Pair.Currency}", LoggerEvent.Sell);
                    break;
            }

            ShowBalance();
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
