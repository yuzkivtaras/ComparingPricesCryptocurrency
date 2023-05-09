using Newtonsoft.Json.Linq;
using WatchListsCryptoMarkets.Client;
using WatchListsCryptoMarkets.IServices;

namespace WatchListsCryptoMarkets.Services.TickerApiService
{
    public class GateIoTickerApiService : ITickerApiService
    {
        private readonly IHttpClientWrapper _httpClient;
        public GateIoTickerApiService(HttpClient httpClient)
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
            var tickerInfo = JArray.Parse(json);

            return tickerInfo;
        }

        public async Task<IEnumerable<string>> GetTickersAsync()
        {
            var tickerInfo = await GetTickerInfoAsync();

            var tickers = from ticker in tickerInfo
                              //select ((string)ticker["id"]).Replace("_", "");
                          select (string)ticker["id"];


            return tickers;
        }
    }
}
