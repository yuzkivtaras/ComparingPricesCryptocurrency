using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services
{
    public class GateioApiService : IGateioApiService
    {
        private readonly IHttpClientWrapper _httpClient;
        public GateioApiService(HttpClient httpClient)
        {
            _httpClient = new HttpClientWrapper(httpClient);
        }

        public async Task<JArray> GetTickerInfoAsync()
        {
            var response = await _httpClient.GetAsync("https://api.gateio.ws/api/v4/spot/currency_pairs");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get ticker pairs from Gate.io API: {response.ReasonPhrase}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var tickerPairs = JArray.Parse(json);

            return JArray.FromObject(tickerPairs);
        }

        public async Task<JArray> GetTickerPricesAsync(List<string> tradingPairs)
        {
            var tickers = new JArray();

            foreach (var pair in tradingPairs)
            {
                var response = await _httpClient.GetAsync($"https://api.gateio.ws/api/v4/spot/tickers?currency_pair={pair}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get ticker prices from Gate.io API for {pair}: {response.ReasonPhrase}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var ticker = JArray.Parse(json);
                tickers.Add(ticker);
            }

            return tickers;
        }
    }
}
