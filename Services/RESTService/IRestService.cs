﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace exmo_trader_bot_console.Services.RESTService
{
    interface IRestService: IStreamService<IRestRequest, IRestResponse>
    {
    }
}
