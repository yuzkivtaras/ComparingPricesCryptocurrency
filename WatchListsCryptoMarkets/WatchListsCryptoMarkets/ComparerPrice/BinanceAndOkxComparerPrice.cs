using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class BinanceAndOkxComparerPrice
    {
        private readonly BinanceTickerApiService _binaceTickerApiService;
        private readonly BinancePriceApiService _binancePriceApiService;
        private readonly OkxTickerApiService _okxTickerApiService;
        private readonly OkxPriceApiService _okxPriceApiService;

        public BinanceAndOkxComparerPrice(BinanceTickerApiService binaceTickerApiService, 
            BinancePriceApiService binancePriceApiService, 
            OkxTickerApiService okxTickerApiService, 
            OkxPriceApiService okxPriceApiService)
        {
            _binaceTickerApiService = binaceTickerApiService;
            _binancePriceApiService = binancePriceApiService;
            _okxTickerApiService = okxTickerApiService;
            _okxPriceApiService = okxPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersBinance = await _binaceTickerApiService.GetTickersAsync();
            var tickersOkx = await _okxTickerApiService.GetTickersAsync();

            var symbolPairs = tickersBinance
                .Where(ticker => tickersOkx.Contains(ReplaceBinanceTickerToOkx(ticker)))
                .Select(ticker => new SymbolPairForBinanceAndOkx
                {
                    BinanceTicker = ticker,
                    OkxTicker = ReplaceBinanceTickerToOkx(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceBinanceTask = _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                var priceOkxTask = _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                await Task.WhenAll(priceBinanceTask, priceOkxTask);

                var priceBinance = priceBinanceTask.Result;
                var priceOkx = priceOkxTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceBinance, priceOkx);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 0.3)
                {
                    var priceBinance = await _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                    var priceOkx = await _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                    Console.WriteLine($"{symbolPair.BinanceTicker}, Difference: {symbolPair.PercentDifference}, Binance: {priceBinance}, OKX: {priceOkx}");
                }
            }
        }

        private string ReplaceBinanceTickerToOkx(string binanceTicker)
        {
            return binanceTicker.Replace("ETH", "-ETH-SWAP")
                .Replace("BTC", "-BTC-SWAP")
                .Replace("USDT", "-USDT-SWAP")
                .Replace("USDC", "-USDC-SWAP");
        }

        private double CalculatePriceDifferencePercent(decimal priceBinance, decimal priceOkx)
        {
            var priceDifference = Math.Abs(priceBinance - priceOkx);
            return (double)priceDifference / (double)priceBinance * 100;
        }
    }

    class SymbolPairForBinanceAndOkx
    {
        public string BinanceTicker { get; set; }
        public string OkxTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
