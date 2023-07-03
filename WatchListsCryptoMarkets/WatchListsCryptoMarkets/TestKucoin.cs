using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IClient;

namespace WatchListsCryptoMarkets
{
    //public class KucoinTickerApiService : ITickerApiService
    //{
    //    private readonly IHttpClientWrapper _httpClient;
    //    public KucoinTickerApiService(HttpClient httpClient)
    //    {
    //        _httpClient = new HttpClientWrapper(httpClient);
    //    }

    //    public async Task<JObject> GetTickerInfoAsync()
    //    {
    //        var response = await _httpClient.GetAsync("https://api.kucoin.com/api/v1/market/allTickers");

    //        if (!response.IsSuccessStatusCode)
    //        {
    //            throw new Exception($"Failed to get ticker pairs from Kucoin API: {response.ReasonPhrase}");
    //        }

    //        var json = await response.Content.ReadAsStringAsync();
    //        var tickerInfo = JObject.Parse(json);

    //        return tickerInfo;
    //    }

    //    public async Task<JArray> GetTickersAsync()
    //    {
    //        var tickerInfo = await GetTickerInfoAsync();

    //        var tickers = (JArray)tickerInfo["data"]["ticker"];

    //        return tickers;
    //    }
    //}
}
