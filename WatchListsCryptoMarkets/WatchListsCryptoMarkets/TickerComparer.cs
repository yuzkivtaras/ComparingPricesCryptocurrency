using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Services;

namespace WatchListsCryptoMarkets
{
    public class TickerComparer
    {
        private readonly IBinanceApiService _binanceApiService;
        private readonly IByBitApiService _byBitApiService;

        public TickerComparer(IBinanceApiService binanceApiService, IByBitApiService byBitApiService)
        {
            _binanceApiService = binanceApiService;
            _byBitApiService = byBitApiService;
        }

        public async Task<List<(string symbol, decimal binancePrice, decimal byBitPrice, decimal priceDiff)>> GetCommonSymbolsAsync()
        {
            var binanceTickerInfo = await _binanceApiService.GetTickerInfoAsync();
            var byBitTickerInfo = await _byBitApiService.GetTickerInfoAsync();

            var commonSymbols = binanceTickerInfo.Select(t => t["symbol"].ToString())
                                .Intersect(byBitTickerInfo.Select(t => t["symbol"].ToString()));

            var results = new List<(string symbol, decimal binancePrice, decimal byBitPrice, decimal priceDiff)>();

            Console.WriteLine("------------Спільні торгові пари:------------");

            foreach (var symbol in commonSymbols)
            {
                var binancePrice = binanceTickerInfo.FirstOrDefault(t => t["symbol"].ToString() == symbol)["price"].ToObject<decimal>();
                var byBitPrice = byBitTickerInfo.FirstOrDefault(t => t["symbol"].ToString() == symbol)["last_price"].ToObject<decimal>();

                var priceDiff = binancePrice - byBitPrice;

                results.Add((symbol, binancePrice, byBitPrice, priceDiff));
            }

            return results.OrderByDescending(r => r.priceDiff).ToList();
        }
    }
}
