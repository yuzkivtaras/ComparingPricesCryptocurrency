using Newtonsoft.Json.Linq;

namespace WatchListsCryptoMarkets.IServices
{
    public interface ITickerApiService
    {
        Task<JArray> GetTickerInfoAsync();
    }
}
