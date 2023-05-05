﻿using Newtonsoft.Json.Linq;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services
{
    public class BinanceApiService : IBinanceApiService
    {
        private readonly IHttpClientWrapper _httpClient;
        public BinanceApiService(HttpClient httpClient)
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
    }
}
