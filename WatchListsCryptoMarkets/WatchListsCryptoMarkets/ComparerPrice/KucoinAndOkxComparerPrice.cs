using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class KucoinAndOkxComparerPrice
    {
        private readonly KucoinTickerApiService _kucoinTickerApiService;
        private readonly KucoinPriceApiService _kucoinPriceApiService;
        private readonly OkxTickerApiService _okxTickerApiService;
        private readonly OkxPriceApiService _okxPriceApiService;

        public KucoinAndOkxComparerPrice(KucoinTickerApiService kucoinTickerApiService, 
            KucoinPriceApiService kucoinPriceApiService, 
            OkxTickerApiService okxTickerApiService, 
            OkxPriceApiService okxPriceApiService)
        {
            _kucoinTickerApiService = kucoinTickerApiService;
            _kucoinPriceApiService = kucoinPriceApiService;
            _okxTickerApiService = okxTickerApiService;
            _okxPriceApiService = okxPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersKucoin = await _kucoinTickerApiService.GetTickersAsync();
            var tickersOkx = await _okxTickerApiService.GetTickersAsync();

            var symbolPairs = tickersKucoin
                .Where(ticker => tickersOkx.Contains(ReplaceKucoinTickerToOkx(ticker)))
                .Select(ticker => new SymbolPairForKucoinAndOkx
                {
                    KucoinTicker = ticker,
                    OkxTicker = ReplaceKucoinTickerToOkx(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceKucoinTask = _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);
                var priceOkxTask = _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                await Task.WhenAll(priceKucoinTask, priceOkxTask);

                var priceKucoin = priceKucoinTask.Result;
                var priceKraken = priceOkxTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceKucoin, priceKraken);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 1)
                {
                    var priceKucoin = await _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);
                    var priceOkx = await _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                    Console.WriteLine($"{symbolPair.KucoinTicker}, Difference: {symbolPair.PercentDifference}, Kucoin: {priceKucoin}, OKX: {priceOkx}");
                }
            }
        }

        private string ReplaceKucoinTickerToOkx(string KucoinTicker)
        {
            return KucoinTicker.Replace("-ETH", "-ETH-SWAP")
                .Replace("-BTC", "-BTC-SWAP")
                .Replace("-USDT", "-USDC-SWAP")
                .Replace("-USDC", "-USDT-SWAP");
        }

        private double CalculatePriceDifferencePercent(decimal priceKucoin, decimal priceOkx)
        {
            var priceDifference = Math.Abs(priceKucoin - priceOkx);
            return (double)priceDifference / (double)priceOkx * 100;
        }
    }

    class SymbolPairForKucoinAndOkx
    {
        public string KucoinTicker { get; set; }
        public string OkxTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
