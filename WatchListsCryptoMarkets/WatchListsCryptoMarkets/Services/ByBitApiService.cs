using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services
{
    public class ByBitApiService : IByBitApiService
    {
        private readonly IHttpClientWrapper _httpClient;
        public ByBitApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
        }

        public async Task<JArray> GetTickerInfoAsync()
        {
            var response = await _httpClient.GetAsync("https://api.bybit.com/v2/public/tickers");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get tickers from ByBit API: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var tickerInfoObj = JObject.Parse(json);
            var tickerInfo = tickerInfoObj["result"].ToObject<JArray>();

            return tickerInfo;
        }
    }
}
