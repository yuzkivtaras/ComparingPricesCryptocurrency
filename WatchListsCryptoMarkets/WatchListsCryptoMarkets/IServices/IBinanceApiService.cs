using Newtonsoft.Json.Linq;

namespace WatchListsCryptoMarkets.IServices
{
    public interface IBinanceApiService
    {
        Task<JArray> GetTickerInfoAsync();
    }
}
