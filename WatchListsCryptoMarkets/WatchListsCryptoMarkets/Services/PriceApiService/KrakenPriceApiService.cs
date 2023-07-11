using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.PriceApiService
{
    public class KrakenPriceApiService : IPriceApiService
    {
        private const string ApiBaseUrl = "https://api.kraken.com/0/public";
        private readonly IHttpClientWrapper _httpClient;
        private readonly SemaphoreSlim _rateLimiter;

        public KrakenPriceApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
            _rateLimiter = new SemaphoreSlim(19);
        }

        public async Task<decimal> GetPriceAsync(string symbol)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var endpoint = $"/Ticker?pair={symbol}";

                var response = await _httpClient.GetAsync($"{ApiBaseUrl}{endpoint}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(content);

                var resultObject = jObject["result"]?[symbol];
                if (resultObject != null && resultObject["c"] != null)
                {
                    var cArray = resultObject["c"] as JArray;
                    if (cArray != null && cArray.Count > 0)
                    {
                        var cValue = cArray[0].ToString();
                        if (decimal.TryParse(cValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal price))
                        {
                            return price;
                        }
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
