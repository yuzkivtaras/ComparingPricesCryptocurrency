using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchListsCryptoMarkets.IServices
{
    public interface IPriceApiService
    {
        Task<decimal> GetPriceAsync(string symbol);
    }
}
