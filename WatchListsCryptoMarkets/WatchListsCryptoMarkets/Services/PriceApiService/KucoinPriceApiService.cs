using Newtonsoft.Json.Linq;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.PriceApiService
{
    public class KucoinPriceApiService : IPriceApiService
    {
        private const string ApiBaseUrl = "https://api.kucoin.com/api/v1";
        private readonly IHttpClientWrapper _httpClient;
        private readonly SemaphoreSlim _rateLimiter;

        public KucoinPriceApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
            _rateLimiter = new SemaphoreSlim(19);
        }

        public async Task<decimal> GetPriceAsync(string symbol)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var endpoint = $"/market/orderbook/level1?symbol={symbol}";

                var response = await _httpClient.GetAsync($"{ApiBaseUrl}{endpoint}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(content);

                if (jObject.TryGetValue("data", out JToken dataToken) && dataToken != null && dataToken.Type == JTokenType.Object)
                {
                    var priceToken = dataToken["price"];
                    if (priceToken != null)
                    {
                        return (decimal)priceToken;
                    }
                }

                return default(decimal);
            }
            finally
            {
                _rateLimiter.Release();
            }
        }
    }
}
