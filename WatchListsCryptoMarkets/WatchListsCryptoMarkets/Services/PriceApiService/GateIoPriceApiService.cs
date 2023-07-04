using Newtonsoft.Json.Linq;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.PriceApiService
{
    public class GateIoPriceApiService : IPriceApiService
    {
        private const string ApiBaseUrl = "https://api.gateio.ws/api/v4";
        private readonly IHttpClientWrapper _httpClient;
        private readonly SemaphoreSlim _rateLimiter;

        public GateIoPriceApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
            _rateLimiter = new SemaphoreSlim(10);
        }

        public async Task<decimal> GetPriceAsync(string symbol)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var endpoint = $"/spot/tickers?currency_pair={symbol}";

                var response = await _httpClient.GetAsync($"{ApiBaseUrl}{endpoint}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jArray = JArray.Parse(content);

                var firstTicker = jArray.First();

                return (decimal)firstTicker["last"];
            }
            finally
            {
                _rateLimiter.Release();
            }
        }
    }
}
