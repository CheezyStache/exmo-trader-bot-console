using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.TradingData;
using exmo_trader_bot_console.Services.Wallet;

namespace exmo_trader_bot_console.Services.OrderResult
{
    class OrderResultService: IOrderResultService
    {
        private readonly IWalletService _walletService;

        public OrderResultService(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public void Subscribe(IObservable<Models.OrderData.OrderResult> inputStream)
        {
            inputStream.Subscribe(ChangeWallet);
        }

        private void ChangeWallet(Models.OrderData.OrderResult orderResult)
        {
            double newValue = 0;

            switch (orderResult.Type)
            {
                case TradeType.Buy:
                    newValue = orderResult.Quantity;
                    break;
                case TradeType.Sell:
                    newValue = orderResult.Amount;
                    break;

                default:
                    throw new NotImplementedException();
            }

            _walletService.ChangeBalance(orderResult.Pair, orderResult.Type, newValue);
        }
    }
}
