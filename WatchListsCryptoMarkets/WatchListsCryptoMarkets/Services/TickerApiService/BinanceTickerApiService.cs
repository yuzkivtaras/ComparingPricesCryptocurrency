using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.TickerApiService
{
    public class BinanceTickerApiService : ITickerApiService
    {
        private readonly IHttpClientWrapper _httpClient;
        public BinanceTickerApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
        }

        public async Task<JArray> GetTickerInfoAsync()
        {
            var response = await _httpClient.GetAsync("https://api.binance.com/api/v3/ticker/price");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get ticker info from Binance API: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var tickerInfo = JArray.Parse(json);

            return tickerInfo;
        }

        public async Task<IEnumerable<string>> GetTickersAsync()
        {
            var tickerInfo = await GetTickerInfoAsync();

            var tickers = from ticker in tickerInfo
                          select (string)ticker["symbol"];

            return tickers;
        }
    }
}
