using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class ByBitAndKucoinComparerPrice
    {
        private readonly ByBitTickerApiService _byBitTickerApiService;
        private readonly ByBitPriceApiService _byBitPriceApiService;
        private readonly KucoinTickerApiService _kucoinTickerApiService;
        private readonly KucoinPriceApiService _kucoinPriceApiService;

        public ByBitAndKucoinComparerPrice(ByBitTickerApiService byBitTickerApiService, 
            ByBitPriceApiService byBitPriceApiService, 
            KucoinTickerApiService kucoinTickerApiService, 
            KucoinPriceApiService kucoinPriceApiService)
        {
            _byBitTickerApiService = byBitTickerApiService;
            _byBitPriceApiService = byBitPriceApiService;
            _kucoinTickerApiService = kucoinTickerApiService;
            _kucoinPriceApiService = kucoinPriceApiService;
        }

        public async Task ComparerPrice()
        {
            string[] differentСurrency = new string[] { "GPTUSDT"};
            string[] unavailableOutputKucoin = new string[] { "ACAUSDT", "ACABTC", "PRIMALUSDT" };
            string[] ilLiquid = new string[] { "TRIBEUSDT", "HEROUSDT", "PLYUSDT" };
            string[] bigRent = new string[] { "SYNRUSDT", "KOKUSDT" };
            string[] differentBlockchains = new string[] { "RUNEUSDT" };

            var tickersByBit = await _byBitTickerApiService.GetTickersAsync();
            var tickersKucoin = await _kucoinTickerApiService.GetTickersAsync();

            var symbolPairs = tickersByBit
                .Where(ticker => tickersKucoin.Contains(ReplaceByBitTickerToKucoin(ticker)) && !differentСurrency.Contains(ticker) && !unavailableOutputKucoin.Contains(ticker) && !ilLiquid.Contains(ticker) && !bigRent.Contains(ticker) && !differentBlockchains.Contains(ticker))
                .Select(ticker => new SymbolPairForByBitAndKucoin
                {
                    ByBitTicker = ticker,
                    KucoinTicker = ReplaceByBitTickerToKucoin(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceByBitTask = _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);
                var priceKucoinTask = _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);

                await Task.WhenAll(priceByBitTask, priceKucoinTask);

                var priceByBit = priceByBitTask.Result;
                var priceKucoin = priceKucoinTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceByBit, priceKucoin);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 5)
                {
                    var priceByBit = await _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);
                    var priceKucoin = await _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);

                    Console.WriteLine($"{symbolPair.ByBitTicker}, Difference: {symbolPair.PercentDifference}, ByBit: {priceByBit}, Kucoin: {priceKucoin}");
                }
            }
        }

        private string ReplaceByBitTickerToKucoin(string ByBitTicker)
        {
            return ByBitTicker.Replace("USDT", "-USDT")
                .Replace("TUSD", "-TUSD")
                .Replace("BUSD", "-BUSD")
                .Replace("USDC", "-USDC")
                .Replace("BNB", "-BNB")
                .Replace("BTC", "-BTC")
                .Replace("ETH", "-ETH")
                .Replace("DAI", "-DAI")
                .Replace("VAI", "-VAI")
                .Replace("XRP", "-XRP")
                .Replace("TRX", "-TRX")
                .Replace("DOGE", "-DOGE")
                .Replace("DOT", "-DOT")
                .Replace("TRY", "-TRY")
                .Replace("EUR", "-EUR")
                .Replace("BRL", "-BRL")
                .Replace("ARS", "-ARS")
                .Replace("BIDR", "-BIDR")
                .Replace("GBP", "-GBP")
                .Replace("IDRT", "-IDRT")
                .Replace("NGN", "-NGN")
                .Replace("PLN", "-PLN")
                .Replace("RUB", "-RUB")
                .Replace("UAH", "-UAH")
                .Replace("ZAR", "-ZAR");
        }

        private double CalculatePriceDifferencePercent(decimal priceByBit, decimal priceKucoin)
        {
            var priceDifference = Math.Abs(priceByBit - priceKucoin);
            return (double)priceDifference / (double)priceByBit * 100;
        }
    }

    class SymbolPairForByBitAndKucoin
    {
        public string ByBitTicker { get; set; }
        public string KucoinTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
