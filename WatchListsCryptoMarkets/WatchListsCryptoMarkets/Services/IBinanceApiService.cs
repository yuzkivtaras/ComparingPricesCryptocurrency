﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchListsCryptoMarkets.Services
{
    public interface IBinanceApiService
    {
        Task<JArray> GetTickerInfoAsync();
    }
}
