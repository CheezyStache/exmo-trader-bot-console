using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Candles;
using exmo_trader_bot_console.Models.Decision;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.Decision
{
    class ResolutionDecisionService : IOutputService<OrderDecision>
    {
        public IObservable<OrderDecision> OutputStream => _orderDecisionSubject;

        private readonly ISubject<OrderDecision> _orderDecisionSubject;
        private readonly DataSettings _dataSettings;
        private readonly int _resolution;
        private readonly IFlowCalcService _flowCalcService;

        public ResolutionDecisionService(DataSettings dataSettings, int resolution)
        {
            _dataSettings = dataSettings;
            _resolution = resolution;

            _flowCalcService = new FlowCalcService();

            _orderDecisionSubject = new Subject<OrderDecision>();
        }

        public void Start(IObservable<Candle[]> candlesStream, IObservable<Trade[]> tradesStream)
        {
            candlesStream.Select(c => MakeDecision(c, tradesStream)).Switch().Subscribe(_orderDecisionSubject);
        }

        public void Stop()
        {
            _orderDecisionSubject.OnCompleted();
        }

        private IObservable<OrderDecision> MakeDecision(Candle[] candles, IObservable<Trade[]> tradesStream)
        {
            var flowLine = _flowCalcService.CalcFlowLine(candles);

            return tradesStream.Select(trades =>
            {
                var currentPos = candles.Length + 1;
                var lastTrade = trades.OrderByDescending(t => t.DateTime).FirstOrDefault();
                if (lastTrade == null) return null;

                var flowLineDiff = _flowCalcService.FlowPercentRange(flowLine, currentPos);
                if (flowLineDiff < _dataSettings.MinDiff) return null;

                var position = _flowCalcService.GetPricePosition(flowLine, lastTrade.Price, currentPos);
                if (position == FlowLinePos.Higher) return CreateDecisionObject(TradeType.MarketSellPrice);
                if (position == FlowLinePos.Lower) return CreateDecisionObject(TradeType.MarketBuyQuantity);

                return null;

            }).Where(d => d != null);
        }

        private OrderDecision CreateDecisionObject(TradeType type)
        {
            var orderDecision = new OrderDecision
            {
                Pair = _dataSettings.Pair,
                Resolution = _resolution,
                Type = type
            };

            return orderDecision;
        }
    }
}
