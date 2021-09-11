using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.DecisionSystems.CandleSignals.Models;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DecisionService;

namespace exmo_trader_bot_console.DecisionSystems.CandleSignals.Services.DecisionService
{
    class CandleSignalsDecisionService: IDecisionService
    {
        public IObservable<OrderDecision> OutputStream => _decisionsSubject;

        private readonly ISubject<OrderDecision> _decisionsSubject;
        private readonly CandleSignalsSettings _settings;
        private readonly TradingPairSettings _pairSettings;
        private double _walletBalanceCurrency;
        private double _walletBalanceCrypto;

        public CandleSignalsDecisionService(IObservable<Trade[][]> candles, CandleSignalsSettings settings, TradingPairSettings pairSettings)
        {
            _settings = settings;
            _pairSettings = pairSettings;
            _walletBalanceCurrency = pairSettings.CurrencyAmount;
            _walletBalanceCrypto = 0;

            _decisionsSubject = new Subject<OrderDecision>();
            candles.Subscribe(MakeDecision);
        }

        public void Subscribe(IObservable<Trade> inputStream)
        {
            throw new NotImplementedException();
        }

        private void MakeDecision(Trade[][] candles)
        {
            var lastIndex = Array.FindLastIndex(candles, c => c.Length < _settings.MinTrades);
            var validCandles = candles.TakeLast(candles.Length - lastIndex - 1);
            var validCandlesCount = validCandles.Count();
            if (validCandlesCount == 0)
                return;

            var validPatterns = _settings.Patterns.Where(p => p.Candles.Length <= validCandlesCount)
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

                if(patternMissed)
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
            if (pattern.Signal == CandleSignal.Buy && _walletBalanceCurrency == 0)
                return;

            if (pattern.Signal == CandleSignal.Sell && _walletBalanceCrypto == 0)
                return;

            var orderDecision = new OrderDecision();
            orderDecision.Description = pattern.Name;
            orderDecision.Pair = _pairSettings.TradingPair;

            if (pattern.Signal == CandleSignal.Buy)
            {
                orderDecision.Type = TradeType.MarketBuyQuantity;
                orderDecision.Quantity = _walletBalanceCurrency;
            }
            else
            {
                orderDecision.Type = TradeType.MarketSellQuantity;
                orderDecision.Quantity = _walletBalanceCrypto;
            }

            _decisionsSubject.OnNext(orderDecision);
        }
    }
}
