﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using exmo_trader_bot_console.Models.Settings;
using RestSharp;

namespace exmo_trader_bot_console.Services.RESTService
{
    class ExmoRestService: RestService
    {
        private Settings _settings;

        public ExmoRestService(Settings settings) : base(settings.Api.OrderCreatePrivate, Method.POST)
        {
            _settings = settings;
        }

        protected override IRestResponse ExecuteRequest(RestRequest restRequest)
        {
            var nonce = DateTime.Now.Ticks;

            restRequest.AddParameter("nonce", nonce.ToString());

            restRequest.AddHeader("Key", _settings.Api.Key);
            restRequest.AddHeader("Sign", _settings.Api.GetSign(nonce));
            restRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            return base.ExecuteRequest(restRequest);
        }
    }
}