using Newtonsoft.Json.Linq;

namespace WatchListsCryptoMarkets.IServices
{
    public interface IGateioApiService
    {
        Task<JArray> GetTickerInfoAsync();
    }
}
