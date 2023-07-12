using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class ByBitAndOkxComparerPrice
    {
        private readonly ByBitTickerApiService _byBitTickerApiService;
        private readonly ByBitPriceApiService _byBitPriceApiService;
        private readonly OkxTickerApiService _okxTickerApiService;
        private readonly OkxPriceApiService _okxPriceApiService;

        public ByBitAndOkxComparerPrice(ByBitTickerApiService byBitTickerApiService, 
            ByBitPriceApiService byBitPriceApiService, 
            OkxTickerApiService okxTickerApiService, 
            OkxPriceApiService okxPriceApiService)
        {
            _byBitTickerApiService = byBitTickerApiService;
            _byBitPriceApiService = byBitPriceApiService;
            _okxTickerApiService = okxTickerApiService;
            _okxPriceApiService = okxPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersByBit = await _byBitTickerApiService.GetTickersAsync();
            var tickersOkx = await _okxTickerApiService.GetTickersAsync();

            var symbolPairs = tickersByBit
                .Where(ticker => tickersOkx.Contains(ReplaceByBitTickerToOkx(ticker)))
                .Select(ticker => new SymbolPairForByBitAndOkx
                {
                    ByBitTicker = ticker,
                    OkxTicker = ReplaceByBitTickerToOkx(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceByBitTask = _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);
                var priceOkxTask = _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                await Task.WhenAll(priceByBitTask, priceOkxTask);

                var priceByBit = priceByBitTask.Result;
                var priceOkx = priceOkxTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceByBit, priceOkx);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 0.3)
                {
                    var priceByBit = await _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);
                    var priceOkx = await _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                    Console.WriteLine($"{symbolPair.ByBitTicker}, Difference: {symbolPair.PercentDifference}, ByBit: {priceByBit}, OKX: {priceOkx}");
                }
            }
        }

        private string ReplaceByBitTickerToOkx(string byBitTicker)
        {
            return byBitTicker.Replace("ETH", "-ETH-SWAP")
                .Replace("BTC", "-BTC-SWAP")
                .Replace("USDT", "-USDT-SWAP")
                .Replace("USDC", "-USDC-SWAP");
        }

        private double CalculatePriceDifferencePercent(decimal priceByBit, decimal priceOkx)
        {
            var priceDifference = Math.Abs(priceByBit - priceOkx);
            return (double)priceDifference / (double)priceByBit * 100;
        }
    }

    class SymbolPairForByBitAndOkx
    {
        public string ByBitTicker { get; set; }
        public string OkxTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
