﻿using Newtonsoft.Json.Linq;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.TickerApiService
{
    public class OkxTickerApiService : ITickerApiService
    {
        private readonly IHttpClientWrapper _httpClient;

        public OkxTickerApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
        }

        public async Task<JArray> GetTickerInfoAsync()
        {
            var response = await _httpClient.GetAsync("https://www.okx.com/api/v5/public/mark-price?instType=SWAP");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get ticker info from Binance API: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var responseObject = JObject.Parse(json);

            var tickerInfo = responseObject["data"] as JArray;

            return tickerInfo;
        }

        public async Task<IEnumerable<string>> GetTickersAsync()
        {
            var tickerInfo = await GetTickerInfoAsync();

            var ignoredTickers = LoadIgnoredTickersFromFile();

            var tickers = from ticker in tickerInfo
                          select (string)ticker["instId"]
                          into symbol
                          where !ignoredTickers.Contains(symbol)
                          select symbol;

            return tickers;
        }

        private IEnumerable<string> LoadIgnoredTickersFromFile()
        {
            var ignoredTickersFile = "D://Repositories/ComparingPricesCryptocurrency/WatchListsCryptoMarkets/WatchListsCryptoMarkets/IgnoreTickers/OkxIgnoreTickers.json";

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
