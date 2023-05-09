using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchListsCryptoMarkets.IServices
{
    public interface ITickerApiService
    {
        Task<JArray> GetTickerInfoAsync();
    }
}
