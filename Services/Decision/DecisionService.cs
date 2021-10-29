using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Decision;
using exmo_trader_bot_console.Models.OrderData;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.DataStorage;
using exmo_trader_bot_console.Services.Settings;

namespace exmo_trader_bot_console.Services.Decision
{
    class DecisionService: IDecisionService
    {
        public IObservable<OrderDecision> OutputStream { get; }

        private readonly IDataStorageService _dataStorageService;
        private readonly SettingsModel _settings;
        private readonly ISubject<OrderDecision> _orderDecisionSubject;
        private readonly IList<PairDecisionService> _pairDecisionServices;

        public DecisionService(IDataStorageService dataStorageService, ISettingsService<SettingsModel> settingsService,
            IObservable<bool> decisionSentStream)
        {
            _dataStorageService = dataStorageService;
            _settings = settingsService.GetSettings();
            _orderDecisionSubject = new Subject<OrderDecision>();
            _pairDecisionServices = new List<PairDecisionService>();

            OutputStream = _orderDecisionSubject.Throttle(decision => decisionSentStream);
        }

        public void Start(IObservable<Trade[]> tradesStream)
        {
            foreach (var pairDecisionService in _pairDecisionServices)
            {
                pairDecisionService.Stop();
            }

            _pairDecisionServices.Clear();

            foreach (var dataSettings in _settings.Data)
            {
                var pairDecisionService = new PairDecisionService(dataSettings);

                foreach (var chart in dataSettings.Chart)
                {
                    var candleResolutionStream = new CandleResolutionStream(chart.Resolution,
                        _dataStorageService.GetCandles(dataSettings.Pair, chart.Resolution));

                    pairDecisionService.Start(candleResolutionStream, tradesStream);
                }

                pairDecisionService.OutputStream.Subscribe(_orderDecisionSubject);
                _pairDecisionServices.Add(pairDecisionService);
            }
        }
    }
}
