using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.PriceApiService
{
    public class BitfinexPriceApiService : IPriceApiService
    {
        private const string ApiBaseUrl = "https://api-pub.bitfinex.com/v2/ticker";
        private readonly IHttpClientWrapper _httpClient;
        private readonly SemaphoreSlim _rateLimiter;

        public BitfinexPriceApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
            _rateLimiter = new SemaphoreSlim(10);
        }

        public async Task<decimal> GetPriceAsync(string symbol)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var endpoint = $"/{symbol}";

                var response = await _httpClient.GetAsync($"{ApiBaseUrl}{endpoint}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jArray = JArray.Parse(content);

                return (decimal)jArray[6];
            }
            finally
            {
                _rateLimiter.Release();
            }           
        }
    }
}
