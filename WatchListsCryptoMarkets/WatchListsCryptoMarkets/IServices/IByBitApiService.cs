using Newtonsoft.Json.Linq;

namespace WatchListsCryptoMarkets.IServices
{
    public interface IByBitApiService
    {
        Task<JArray> GetTickerInfoAsync();
    }
}
