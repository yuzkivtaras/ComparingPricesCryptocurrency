using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class BinanceAndKucoinComparerPrice
    {
        private readonly BinanceTickerApiService _binaceTickerApiService;
        private readonly BinancePriceApiService _binancePriceApiService;
        private readonly KucoinTickerApiService _kucoinTickerApiService;
        private readonly KucoinPriceApiService _kucoinPriceApiService;

        public BinanceAndKucoinComparerPrice(BinanceTickerApiService binaceTickerApiService, 
            BinancePriceApiService binancePriceApiService, 
            KucoinTickerApiService kucoinTickerApiService, 
            KucoinPriceApiService kucoinPriceApiService)
        {
            _binaceTickerApiService = binaceTickerApiService;
            _binancePriceApiService = binancePriceApiService;
            _kucoinTickerApiService = kucoinTickerApiService;
            _kucoinPriceApiService = kucoinPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersBinance = await _binaceTickerApiService.GetTickersAsync();
            var tickersKucoin = await _kucoinTickerApiService.GetTickersAsync();

            var symbolPairs = tickersBinance
                .Where(ticker => tickersKucoin.Contains(ReplaceBinanceTickerToKucoin(ticker)))
                .Select(ticker => new SymbolPairForBinanceAndKucoin
                {
                    BinanceTicker = ticker,
                    KucoinTicker = ReplaceBinanceTickerToKucoin(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceBinanceTask = _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                var priceKucoinTask = _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);

                await Task.WhenAll(priceBinanceTask, priceKucoinTask);

                var priceBinance = priceBinanceTask.Result;
                var priceKucoin = priceKucoinTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceBinance, priceKucoin);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 1.5)
                {
                    var priceBinance = await _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                    var priceKucoin = await _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);

                    Console.WriteLine($"{symbolPair.BinanceTicker}, Difference: {symbolPair.PercentDifference}, Binance: {priceBinance}, Kucoin: {priceKucoin}");       
                }
            }
        }

        private string ReplaceBinanceTickerToKucoin(string binanceTicker)
        {
            return binanceTicker.Replace("ETH", "-ETH")
                .Replace("BTC", "-BTC")
                .Replace("USDT", "-USDT");
        }

        private double CalculatePriceDifferencePercent(decimal priceBinance, decimal priceKucoin)
        {
            var priceDifference = Math.Abs(priceBinance - priceKucoin);
            return (double)priceDifference / (double)priceBinance * 100;
        }
    }

    class SymbolPairForBinanceAndKucoin
    {
        public string BinanceTicker { get; set; }
        public string KucoinTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
