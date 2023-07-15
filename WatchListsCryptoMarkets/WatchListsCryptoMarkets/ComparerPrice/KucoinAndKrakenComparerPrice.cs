using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class KucoinAndKrakenComparerPrice
    {
        private readonly KucoinTickerApiService _kucoinTickerApiService;
        private readonly KucoinPriceApiService _kucoinPriceApiService;
        private readonly KrakenTicketApiService _krakenTicketApiService;
        private readonly KrakenPriceApiService _krakenPriceApiService;

        public KucoinAndKrakenComparerPrice(KucoinTickerApiService kucoinTickerApiService, 
            KucoinPriceApiService kucoinPriceApiService, 
            KrakenTicketApiService rakenTicketApiService, 
            KrakenPriceApiService rakenPriceApiService)
        {
            _kucoinTickerApiService = kucoinTickerApiService;
            _kucoinPriceApiService = kucoinPriceApiService;
            _krakenTicketApiService = rakenTicketApiService;
            _krakenPriceApiService = rakenPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersKucoin = await _kucoinTickerApiService.GetTickersAsync();
            var tickersKraken = await _krakenTicketApiService.GetTickersAsync();

            var symbolPairs = tickersKucoin
                .Where(ticker => tickersKraken.Contains(ReplaceKucoinTickerToKraken(ticker)))
                .Select(ticker => new SymbolPairForKucoinAndKraken
                {
                   KucoinTicker = ticker,
                   KrakenTicker = ReplaceKucoinTickerToKraken(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {                
                var priceKucoinTask = _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);
                var priceKrakenTask = _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);

                await Task.WhenAll(priceKucoinTask, priceKrakenTask);
                
                var priceKucoin = priceKucoinTask.Result;
                var priceKraken = priceKrakenTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceKucoin, priceKraken);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 4)
                {                    
                    var priceKucoin = await _kucoinPriceApiService.GetPriceAsync(symbolPair.KucoinTicker);
                    var priceKraken = await _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);

                    Console.WriteLine($"{symbolPair.KucoinTicker}, Difference: {symbolPair.PercentDifference}, Kucoin: {priceKucoin}, Kraken: {priceKraken}");
                }
            }
        }

        private string ReplaceKucoinTickerToKraken(string KucoinTicker)
        {
            return KucoinTicker.Replace("-USDT", "/USDT")
                .Replace("-TUSD", "/TUSD")
                .Replace("-BUSD", "/BUSD")
                .Replace("-USDC", "/USDC")
                .Replace("-BNB", "/BNB")
                .Replace("-BTC", "/BTC")
                .Replace("-ETH", "/ETH")
                .Replace("-DAI", "/DAI")
                .Replace("-VAI", "/VAI")
                .Replace("-XRP", "/XRP")
                .Replace("-TRX", "/TRX")
                .Replace("-DOGE", "/DOGE")
                .Replace("-DOT", "/DOT")
                .Replace("-TRY", "/TRY")
                .Replace("-EUR", "/EUR")
                .Replace("-BRL", "/BRL")
                .Replace("-ARS", "/ARS")
                .Replace("-BIDR", "/BIDR")
                .Replace("-GBP", "/GBP")
                .Replace("-IDRT", "/IDRT")
                .Replace("-NGN", "/NGN")
                .Replace("-PLN", "/PLN")
                .Replace("-RUB", "/RUB")
                .Replace("-UAH", "/UAH")
                .Replace("-ZAR", "/ZAR");
        }

        private double CalculatePriceDifferencePercent(decimal priceKucoin, decimal priceKraken)
        {
            var priceDifference = Math.Abs(priceKucoin - priceKraken);
            return (double)priceDifference / (double)priceKraken * 100;
        }
    }

    class SymbolPairForKucoinAndKraken
    {       
        public string KucoinTicker { get; set; }
        public string KrakenTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
