using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class BinanceAndKrakenComparerPrice
    {
        private readonly BinanceTickerApiService _binaceTickerApiService;
        private readonly BinancePriceApiService _binancePriceApiService;
        private readonly KrakenTicketApiService _krakenTicketApiService;
        private readonly KrakenPriceApiService _krakenPriceApiService;

        public BinanceAndKrakenComparerPrice(BinanceTickerApiService binaceTickerApiService, 
            BinancePriceApiService binancePriceApiService, 
            KrakenTicketApiService krakenTicketApiService, 
            KrakenPriceApiService krakenPriceApiService)
        {
            _binaceTickerApiService = binaceTickerApiService;
            _binancePriceApiService = binancePriceApiService;
            _krakenTicketApiService = krakenTicketApiService;
            _krakenPriceApiService = krakenPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersBinance = await _binaceTickerApiService.GetTickersAsync();
            var tickersKraken = await _krakenTicketApiService.GetTickersAsync();

            var symbolPairs = tickersBinance
                .Where(ticker => tickersKraken.Contains(ReplaceBinanceTickerToKraken(ticker)))
                .Select(ticker => new SymbolPairForBinanceAndKraken
                {
                    BinanceTicker = ticker,
                    KrakenTicker = ReplaceBinanceTickerToKraken(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceBinanceTask = _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                var priceKrakenTask = _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);

                await Task.WhenAll(priceBinanceTask, priceKrakenTask);

                var priceBinance = priceBinanceTask.Result;
                var priceKraken = priceKrakenTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceBinance, priceKraken);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 5)
                {
                    var priceBinance = await _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                    var priceKraken = await _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);

                    Console.WriteLine($"{symbolPair.BinanceTicker}, Difference: {symbolPair.PercentDifference}, Binance: {priceBinance}, Kraken: {priceKraken}");
                }
            }
        }

        private string ReplaceBinanceTickerToKraken(string binanceTicker)
        {
            return binanceTicker.Replace("USDT", "/USDT")
                .Replace("TUSD", "/TUSD")
                .Replace("BUSD", "/BUSD")
                .Replace("USDC", "/USDC")
                .Replace("BNB", "/BNB")
                .Replace("BTC", "/BTC")
                .Replace("ETH", "/ETH")
                .Replace("DAI", "/DAI")
                .Replace("VAI", "/VAI")
                .Replace("XRP", "/XRP")
                .Replace("TRX", "/TRX")
                .Replace("DOGE", "/DOGE")
                .Replace("DOT", "/DOT")
                .Replace("TRY", "/TRY")
                .Replace("EUR", "/EUR")
                .Replace("BRL", "/BRL")
                .Replace("ARS", "/ARS")
                .Replace("BIDR", "/BIDR")
                .Replace("GBP", "/GBP")
                .Replace("IDRT", "/IDRT")
                .Replace("NGN", "/NGN")
                .Replace("PLN", "/PLN")
                .Replace("RUB", "/RUB")
                .Replace("UAH", "/UAH")
                .Replace("ZAR", "/ZAR");
        }

        private double CalculatePriceDifferencePercent(decimal priceBinance, decimal priceKraken)
        {
            var priceDifference = Math.Abs(priceBinance - priceKraken);
            return (double)priceDifference / (double)priceBinance * 100;
        }
    }

    class SymbolPairForBinanceAndKraken
    {
        public string BinanceTicker { get; set; }
        public string KrakenTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
