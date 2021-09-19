using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services;
using exmo_trader_bot_console.Services.WalletService;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService
{
    class PatternCheckService: IStreamService<Trade[][], OrderDecision>
    {
        public IObservable<OrderDecision> OutputStream => _decisionsSubject;

        private readonly ISubject<OrderDecision> _decisionsSubject;
        private readonly CandleSignalsSettings _settings;
        private readonly IEnumerable<CandlePattern> _candlePatterns;
        private readonly TradingPair _pair;
        private readonly IWalletService _walletService;

        public PatternCheckService(CandleSignalsSettings settings, IEnumerable<CandlePattern> candlePatterns,
            TradingPair tradingPair, IWalletService walletService)
        {
            _decisionsSubject = new Subject<OrderDecision>();

            _settings = settings;
            _pair = tradingPair;
            _candlePatterns = candlePatterns;

            _walletService = walletService;
        }

        public void Subscribe(IObservable<Trade[][]> inputStream)
        {
            inputStream.Subscribe(MakeDecision);
        }

        private void MakeDecision(Trade[][] candles)
        {
            var lastIndex = Array.FindLastIndex(candles, c => c.Length < _settings.MinTrades);
            var validCandles = candles.TakeLast(candles.Length - lastIndex - 1);
            var validCandlesCount = validCandles.Count();
            if (validCandlesCount == 0)
                return;

            var validPatterns = _candlePatterns.Where(p => p.Candles.Length <= validCandlesCount)
                .OrderByDescending(p => p.Candles.Length);

            foreach (var pattern in validPatterns)
            {
                var checkCandles = validCandles.TakeLast(pattern.Candles.Length)
                    .ToArray();

                var patternMissed = false;

                for (int i = 0; i < pattern.Candles.Length; i++)
                {
                    var trades = checkCandles[i].OrderBy(c => c.DateTime)
                        .ToArray();
                    var candleProps = CalculateCandle(trades);

                    var result = CheckPattern(candleProps, pattern.Candles[i]);
                    if (!result)
                    {
                        patternMissed = true;
                        break;
                    }
                }

                if (patternMissed)
                    continue;

                ConstructDecision(pattern);
                break;
            }
        }

        private CandleProps CalculateCandle(Trade[] trades)
        {
            var candleProps = new CandleProps();

            var min = trades.Min(t => t.Price);
            var max = trades.Max(t => t.Price);

            var openPrice = trades[0].Price;
            var closePrice = trades[^1].Price;

            if (openPrice > closePrice)
                candleProps.Movement = CandleMovement.Red;
            else if (openPrice < closePrice)
                candleProps.Movement = CandleMovement.Green;
            else
                candleProps.Movement = CandleMovement.White;

            var maxBodyPrice = openPrice > closePrice ? openPrice : closePrice;
            var minBodyPrice = openPrice > closePrice ? closePrice : openPrice;

            candleProps.UpperShadowPercent = (max - maxBodyPrice) / (max - min) * 100;
            candleProps.LowerShadowPercent = (minBodyPrice - min) / (max - min) * 100;

            return candleProps;
        }

        private bool CheckPattern(CandleProps candleProps, CandleProps patternProps)
        {
            if (candleProps.Movement != patternProps.Movement)
                return false;

            if (Math.Abs(candleProps.UpperShadowPercent - patternProps.UpperShadowPercent) > _settings.ErrorPercent)
                return false;

            if (Math.Abs(candleProps.LowerShadowPercent - patternProps.LowerShadowPercent) > _settings.ErrorPercent)
                return false;

            return true;
        }

        private void ConstructDecision(CandlePattern pattern)
        {
            var currencyBalance = _walletService.Wallet[_pair].Currency;
            var cryptoBalance = _walletService.Wallet[_pair].Crypto;

            if (pattern.Signal == CandleSignal.Buy && currencyBalance == 0)
                return;

            if (pattern.Signal == CandleSignal.Sell && cryptoBalance == 0)
                return;

            var orderDecision = new OrderDecision();
            orderDecision.Description = pattern.Name;
            orderDecision.Pair = _pair;

            if (pattern.Signal == CandleSignal.Buy)
            {
                orderDecision.Type = TradeType.MarketBuyQuantity;
                orderDecision.Quantity = currencyBalance;
            }
            else
            {
                orderDecision.Type = TradeType.MarketSellQuantity;
                orderDecision.Quantity = cryptoBalance;
            }

            _decisionsSubject.OnNext(orderDecision);
        }
    }
}
