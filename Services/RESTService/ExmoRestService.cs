using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using exmo_trader_bot_console.Services.Settings;
using RestSharp;

namespace exmo_trader_bot_console.Services.RESTService
{
    class ExmoRestService: RestService
    {
        private Models.Settings.Settings _settings;

        public ExmoRestService(ISettingsService<Models.Settings.Settings> settingsService) : base(
            settingsService.GetSettings().Api.OrderCreatePrivate, Method.POST)
        {
            _settings = settingsService.GetSettings();
        }

        protected override string ExecuteRequest(IRestRequest restRequest)
        {
            var nonce = DateTime.Now.Ticks;
            restRequest.AddParameter("nonce", nonce.ToString());

            var content = string.Join("&", restRequest.Parameters.Select(p => p.Name + "=" + p.Value));
            restRequest.AddHeader("Sign", _settings.Api.GetSign(content));

            restRequest.AddHeader("Key", _settings.Api.Key);
            restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            return base.ExecuteRequest(restRequest);
        }
    }
}
