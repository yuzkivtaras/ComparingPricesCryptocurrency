using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class KrakenAndOkxComparerPrice
    {
        private readonly KrakenTicketApiService _krakenTicketApiService;
        private readonly KrakenPriceApiService _krakenPriceApiService;
        private readonly OkxTickerApiService _okxTickerApiService;
        private readonly OkxPriceApiService _okxPriceApiService;

        public KrakenAndOkxComparerPrice(KrakenTicketApiService krakenTicketApiService, 
            KrakenPriceApiService krakenPriceApiService, 
            OkxTickerApiService okxTickerApiService, 
            OkxPriceApiService okxPriceApiService)
        {
            _krakenTicketApiService = krakenTicketApiService;
            _krakenPriceApiService = krakenPriceApiService;
            _okxTickerApiService = okxTickerApiService;
            _okxPriceApiService = okxPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersKraken = await _krakenTicketApiService.GetTickersAsync();
            var tickersOkx = await _okxTickerApiService.GetTickersAsync();

            var symbolPairs = tickersKraken
                .Where(ticker => tickersOkx.Contains(ReplaceKrakenTickerToOkx(ticker)))
                .Select(ticker => new SymbolPairForKrakenAndOkx
                {
                    KrakenTicker = ticker,
                    OkxTicker = ReplaceKrakenTickerToOkx(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceKrakenTask = _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);
                var priceOkxTask = _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                await Task.WhenAll(priceKrakenTask, priceOkxTask);

                var priceKraken = priceKrakenTask.Result;
                var priceOkx = priceOkxTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceKraken, priceOkx);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 5)
                {                    
                    var priceKraken = await _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);
                    var priceOkx = await _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                    Console.WriteLine($"{symbolPair.KrakenTicker}, Difference: {symbolPair.PercentDifference}, Kraken: {priceKraken}, OKX: {priceOkx}");
                }
            }
        }

        private string ReplaceKrakenTickerToOkx(string KrakenTicker)
        {
            return KrakenTicker.Replace("/USDT", "-USDT-SWAP")
                .Replace("/TUSD", "-TUSD-SWAP")
                .Replace("/BUSD", "-BUSD-SWAP")
                .Replace("/USDC", "-USDC-SWAP")
                .Replace("/BNB", "-BNB-SWAP")
                .Replace("/BTC", "-BTC-SWAP")
                .Replace("/ETH", "-ETH-SWAP")
                .Replace("/DAI", "-DAI-SWAP")
                .Replace("/VAI", "-VAI-SWAP")
                .Replace("/XRP", "-XRP-SWAP")
                .Replace("/TRX", "-TRX-SWAP")
                .Replace("/DOGE", "-DOGE-SWAP")
                .Replace("/DOT", "-DOT-SWAP")
                .Replace("/TRY", "-TRY-SWAP")
                .Replace("/EUR", "-EUR-SWAP")
                .Replace("/BRL", "-BRL-SWAP")
                .Replace("/ARS", "-ARS-SWAP")
                .Replace("/BIDR", "-BIDR-SWAP")
                .Replace("/GBP", "-GBP-SWAP")
                .Replace("/IDRT", "-IDRT-SWAP")
                .Replace("/NGN", "-NGN-SWAP")
                .Replace("/PLN", "-PLN-SWAP")
                .Replace("/RUB", "-RUB-SWAP")
                .Replace("/UAH", "-UAH-SWAP")
                .Replace("/ZAR", "-ZAR-SWAP");
        }

        private double CalculatePriceDifferencePercent(decimal priceKraken, decimal priceOkx)
        {
            var priceDifference = Math.Abs(priceKraken - priceOkx);
            return (double)priceDifference / (double)priceOkx * 100;
        }
    }

    class SymbolPairForKrakenAndOkx
    {
        public string KrakenTicker { get; set; }
        public string OkxTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
