using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class BinanceAndGateIoComparerPrice
    {
        private readonly BinanceTickerApiService _binaceTickerApiService;
        private readonly BinancePriceApiService _binancePriceApiService;
        private readonly GateIoTickerApiService _gateIoTickerApiService;
        private readonly GateIoPriceApiService _gateIoPriceApiService;

        public BinanceAndGateIoComparerPrice(BinanceTickerApiService binaceTickerApiService,
            BinancePriceApiService binancePriceApiService,
            GateIoTickerApiService gateIoTickerApiService,
            GateIoPriceApiService gateIoPriceApiService)
        {
            _binaceTickerApiService = binaceTickerApiService;
            _binancePriceApiService = binancePriceApiService;
            _gateIoTickerApiService = gateIoTickerApiService;
            _gateIoPriceApiService = gateIoPriceApiService;
        }

        public async Task ComparerPrice()
        {
            string[] justForGateIoTickers = new string[] { "BIFIUSDT", "GTCUSDT", "GTCBTC" };
            string[] unavailableOutputGateIo = new string[] { "TORNUSDT", "MLNUSDT", "IOTABTC", "FLMUSDT", "IOTAUSDT",  };
            string[] differentСurrency = new string[] { "QIUSDT" };
            string[] differentBlockchains = new string[] { "MDXUSDT" };
            string[] bigRent = new string[] { "DEGOUSDT" };

            var tickersBinance = await _binaceTickerApiService.GetTickersAsync();
            var tickersGateIo = await _gateIoTickerApiService.GetTickersAsync();

            var symbolPairs = tickersBinance
                .Where(ticker => tickersGateIo.Contains(ReplaceBinanceTickerToGateIo(ticker)) && !justForGateIoTickers.Contains(ticker) && !unavailableOutputGateIo.Contains(ticker) && !differentBlockchains.Contains(ticker) && !bigRent.Contains(ticker))
                .Select(ticker => new SymbolPairForBinanceAndGateIo
                {
                    BinanceTicker = ticker,
                    GateIoTicker = ReplaceBinanceTickerToGateIo(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceBinanceTask = _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                var priceGateIoTask = _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);

                await Task.WhenAll(priceBinanceTask, priceGateIoTask);

                var priceBinance = priceBinanceTask.Result;
                var priceGateIo = priceGateIoTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceBinance, priceGateIo);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 3.5)
                {
                    var priceBinance = await _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                    var priceGateIo = await _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);

                    Console.WriteLine($"{symbolPair.BinanceTicker}, Difference: {symbolPair.PercentDifference}, Binance: {priceBinance}, GateIo: {priceGateIo}");
                }
            }
        }

        private string ReplaceBinanceTickerToGateIo(string binanceTicker)
        {
            return binanceTicker.Replace("USDT", "_USDT")
                .Replace("TUSD", "_TUSD")
                .Replace("BUSD", "_BUSD")
                .Replace("USDC", "_USDC")
                .Replace("BNB", "_BNB")
                .Replace("BTC", "_BTC")
                .Replace("ETH", "_ETH")
                .Replace("DAI", "_DAI")
                .Replace("VAI", "_VAI")
                .Replace("XRP", "_XRP")
                .Replace("TRX", "_TRX")
                .Replace("DOGE", "_DOGE")
                .Replace("DOT", "_DOT")
                .Replace("TRY", "_TRY")
                .Replace("EUR", "_EUR")
                .Replace("BRL", "_BRL")
                .Replace("ARS", "_ARS")
                .Replace("BIDR", "_BIDR")
                .Replace("GBP", "_GBP")
                .Replace("IDRT", "_IDRT")
                .Replace("NGN", "_NGN")
                .Replace("PLN", "_PLN")
                .Replace("RUB", "_RUB")
                .Replace("UAH", "_UAH")
                .Replace("ZAR", "_ZAR");
        }

        private double CalculatePriceDifferencePercent(decimal priceBinance, decimal priceGateIo)
        {
            var priceDifference = Math.Abs(priceBinance - priceGateIo);
            return (double)priceDifference / (double)priceBinance * 100;
        }
    }

    class SymbolPairForBinanceAndGateIo
    {
        public string BinanceTicker { get; set; }
        public string GateIoTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
