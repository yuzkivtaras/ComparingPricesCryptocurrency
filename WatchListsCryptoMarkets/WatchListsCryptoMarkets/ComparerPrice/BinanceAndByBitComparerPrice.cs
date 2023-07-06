using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class BinanceAndByBitComparerPrice
    {
        private readonly BinanceTickerApiService _binaceTickerApiService;
        private readonly BinancePriceApiService _binancePriceApiService;
        private readonly ByBitTickerApiService _byBitTickerApiService;
        private readonly ByBitPriceApiService _byBitPriceApiService;

        public BinanceAndByBitComparerPrice(BinanceTickerApiService binaceTickerApiService, 
            BinancePriceApiService binancePriceApiService, 
            ByBitTickerApiService byBitTickerApiService, 
            ByBitPriceApiService byBitPriceApiService)
        {
            _binaceTickerApiService = binaceTickerApiService;
            _binancePriceApiService = binancePriceApiService;
            _byBitTickerApiService = byBitTickerApiService;
            _byBitPriceApiService = byBitPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersBinance = await _binaceTickerApiService.GetTickersAsync();
            var tickersByBit = await _byBitTickerApiService.GetTickersAsync();

            var symbolPairs = tickersBinance
                .Where(ticker => tickersByBit.Contains(ticker))
                .Select(ticker => new SymbolPairForBinanceAndByBit
                {
                    BinanceTicker = ticker,
                    ByBitTicker = ticker
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceBinanceTask = _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                var priceByBitTask = _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);

                await Task.WhenAll(priceBinanceTask, priceByBitTask);

                var priceBinance = await priceBinanceTask;
                var priceByBit = await priceByBitTask;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceBinance, priceByBit);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 3)
                {
                    var priceBinance = await _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                    var priceByBit = await _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);

                    Console.WriteLine($"{symbolPair.BinanceTicker}, Difference: {symbolPair.PercentDifference}, Binance: {priceBinance}, ByBit: {priceByBit}");
                }
            }
        }

        private double CalculatePriceDifferencePercent(decimal priceBinance, decimal priceByBit)
        {
            var priceDifference = Math.Abs(priceBinance - priceByBit);
            return (double)priceDifference / (double)priceBinance * 100;
        }
    }

    class SymbolPairForBinanceAndByBit
    {
        public string BinanceTicker { get; set; }
        public string ByBitTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
