using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class GateIoAndKucoinComparerPrice
    {
        private readonly GateIoTickerApiService _gateIoTickerApiService;
        private readonly GateIoPriceApiService _gateIoPriceApiService;
        private readonly KucoinTickerApiService _kucoinTickerApiService;
        private readonly KucoinPriceApiService _kucoinPriceApiService;

        public GateIoAndKucoinComparerPrice(GateIoTickerApiService gateIoTickerApiService, 
            GateIoPriceApiService gateIoPriceApiService, 
            KucoinTickerApiService kucoinTickerApiService, 
            KucoinPriceApiService kucoinPriceApiService)
        {
            _gateIoTickerApiService = gateIoTickerApiService;
            _gateIoPriceApiService = gateIoPriceApiService;
            _kucoinTickerApiService = kucoinTickerApiService;
            _kucoinPriceApiService = kucoinPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersGateIo = await _gateIoTickerApiService.GetTickersAsync();
            var tickersKucoin = await _kucoinTickerApiService.GetTickersAsync();

            var symbolPairs = tickersGateIo
                .Where(ticker => tickersKucoin.Contains(ReplaceGateIoTickerToKucoin(ticker)))
                .Select(ticker => new SymbolPairForGateIoAndKucoin
                {
                    GateIoTicker = ticker,
                    KucoinTicker = ReplaceGateIoTickerToKucoin(ticker),
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {                
                var priceGateIoTask = _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);
                var priceKucoinTask = _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);

                await Task.WhenAll(priceGateIoTask, priceKucoinTask);
      
                var priceGateIo = priceGateIoTask.Result;
                var priceKucoin = priceKucoinTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceGateIo, priceKucoin);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 3)
                {
                    var priceGateIo = await _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);
                    var priceKucoin = await _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);                    

                    Console.WriteLine($"{symbolPair.GateIoTicker}, Difference: {symbolPair.PercentDifference}, GateIo: {priceGateIo}, Kucoin: {priceKucoin}");
                }
            }
        }

        private string ReplaceGateIoTickerToKucoin(string gateIoTicker)
        {
            return gateIoTicker.Replace("_ETH", "-ETH")
                .Replace("_BTC", "-BTC")
                .Replace("_USDT", "-USDT")
                .Replace("_USDC", "-USDC");
        }

        private double CalculatePriceDifferencePercent(decimal priceGateIo, decimal priceKucoin)
        {
            var priceDifference = Math.Abs(priceGateIo - priceKucoin);
            return (double)priceDifference / (double)priceGateIo * 100;
        }
    }

    class SymbolPairForGateIoAndKucoin
    {
        public string GateIoTicker { get; set; }
        public string KucoinTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
