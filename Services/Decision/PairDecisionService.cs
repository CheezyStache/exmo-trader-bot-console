using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Decision;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;

namespace exmo_trader_bot_console.Services.Decision
{
    class PairDecisionService: IOutputService<OrderDecision>
    {
        public IObservable<OrderDecision> OutputStream => _orderDecisionSubject;

        private readonly ISubject<OrderDecision> _orderDecisionSubject;
        private readonly DataSettings _dataSettings;
        private readonly IList<ResolutionDecisionService> _resolutionDecisionServices;

        public PairDecisionService(DataSettings dataSettings)
        {
            _orderDecisionSubject = new Subject<OrderDecision>();
            _dataSettings = dataSettings;
            _resolutionDecisionServices = new List<ResolutionDecisionService>();
        }

        public void Start(CandleResolutionStream candleResolutionStream, IObservable<Trade[]> tradesStream)
        {
            var resolutionDecisionService =
                new ResolutionDecisionService(_dataSettings, candleResolutionStream.Resolution);

            resolutionDecisionService.Start(candleResolutionStream.Candles, tradesStream);
            resolutionDecisionService.OutputStream.Subscribe(_orderDecisionSubject);

            _resolutionDecisionServices.Add(resolutionDecisionService);
        }

        public void Stop()
        {
            foreach (var resolutionDecisionService in _resolutionDecisionServices)
            {
                resolutionDecisionService.Stop();
            }

            _resolutionDecisionServices.Clear();

            _orderDecisionSubject.OnCompleted();
        }
    }
}
