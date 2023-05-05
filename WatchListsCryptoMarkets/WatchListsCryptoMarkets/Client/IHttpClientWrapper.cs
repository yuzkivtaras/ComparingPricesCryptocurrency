

namespace WatchListsCryptoMarkets.Client
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }
}
