using Newtonsoft.Json.Linq;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.PriceApiService
{
    public class BinancePriceApiService : IPriceApiService
    {
        private const string ApiBaseUrl = "https://api.binance.com/api/v3";
        private readonly IHttpClientWrapper _httpClient;
        public BinancePriceApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
        }

        public async Task<decimal> GetPriceAsync(string symbol)
        {
            var endpoint = $"/ticker/price?symbol={symbol}";

            var response = await _httpClient.GetAsync($"{ApiBaseUrl}{endpoint}");

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(content);

            return (decimal)jObject["price"];
        }
    }
}
