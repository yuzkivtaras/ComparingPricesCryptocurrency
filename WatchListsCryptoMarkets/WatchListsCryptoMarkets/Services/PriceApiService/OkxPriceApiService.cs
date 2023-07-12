using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.PriceApiService
{
    public class OkxPriceApiService : IPriceApiService
    {
        private const string ApiBaseUrl = "https://www.okx.com";
        private readonly IHttpClientWrapper _httpClient;
        private readonly SemaphoreSlim _rateLimiter;

        public OkxPriceApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
            _rateLimiter = new SemaphoreSlim(9);
        }

        public async Task<decimal> GetPriceAsync(string symbol)
        {
            await _rateLimiter.WaitAsync();

            try
            {
                var endpoint = $"/api/v5/public/mark-price?instId={symbol}";

                var response = await _httpClient.GetAsync($"{ApiBaseUrl}{endpoint}");

                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var jObject = JObject.Parse(content);

                if (jObject.ContainsKey("data") && jObject["data"].HasValues)
                {
                    var markPx = (string)jObject["data"][0]["markPx"];
                    decimal price;

                    var markPxFormatted = markPx.Replace(",", ".");
                    if (decimal.TryParse(markPxFormatted, NumberStyles.Float, CultureInfo.InvariantCulture, out price))
                    {
                        return price;
                    }
                }

                return (decimal)default;

            }
            finally
            {
                _rateLimiter.Release();
            }
        }
    }
}
