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
    public class BitfinexTickerApiService : ITickerApiService
    {
        private readonly IHttpClientWrapper _httpClient;

        public BitfinexTickerApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
        }

        public async Task<JArray> GetTickerInfoAsync()
        {
            var response = await _httpClient.GetAsync("https://api-pub.bitfinex.com/v2/tickers?symbols=ALL");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get ticker info from Bitfinex API: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();

            var tickerInfo = JArray.Parse(json);

            return tickerInfo;
        }

        public async Task<IEnumerable<string>> GetTickersAsync()
        {
            var tickerInfo = await GetTickerInfoAsync();

            var ignoredTickers = LoadIgnoredTickersFromFile();

            var tickers = from tickerArray in tickerInfo
                          select (string)tickerArray[0]
                          into symbol
                          where !ignoredTickers.Contains(symbol)
                          select symbol;

            return tickers;
        }

        private IEnumerable<string> LoadIgnoredTickersFromFile()
        {
            var ignoredTickersFile = "D://Repositories/ComparingPricesCryptocurrency/WatchListsCryptoMarkets/WatchListsCryptoMarkets/IgnoreTickers/BitfinexIgnoreTickers.json";

            if (File.Exists(ignoredTickersFile))
            {
                var json = File.ReadAllText(ignoredTickersFile);
                var ignoredTickers = JArray.Parse(json).ToObject<List<string>>();
                return ignoredTickers;
            }

            return Enumerable.Empty<string>();
        }
    }
}
