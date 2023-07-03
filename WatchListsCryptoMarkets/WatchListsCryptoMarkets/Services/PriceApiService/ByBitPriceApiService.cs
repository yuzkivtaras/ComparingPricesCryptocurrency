using Newtonsoft.Json.Linq;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.PriceApiService
{
    public class ByBitPriceApiService : IPriceApiService
    {
        private const string ApiBaseUrl = "https://api.bybit.com/spot/v3";
        private readonly IHttpClientWrapper _httpClient;
        private readonly SemaphoreSlim _rateLimiter;

        public ByBitPriceApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
            _rateLimiter = new SemaphoreSlim(19);
        }



        public async Task<decimal> GetPriceAsync(string symbol)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var endpoint = $"/public/quote/ticker/price?symbol={symbol}";

                var response = await _httpClient.GetAsync($"{ApiBaseUrl}{endpoint}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(content);

                return (decimal)jObject["result"]["price"];
            }
            finally
            {
                _rateLimiter.Release();
            }
        }
    }
}
