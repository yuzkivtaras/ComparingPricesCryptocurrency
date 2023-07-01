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
            string[] tickersDiscribe = new string[] { "MLN_USDT", "IOTA_USDT" };

            var tickersBinance = await _binaceTickerApiService.GetTickersAsync();
            var tickersGateIo = await _gateIoTickerApiService.GetTickersAsync();

            var symbolPairs = tickersBinance
                .Where(ticker => tickersGateIo.Contains(ReplaceBinanceTickerToGateIo(ticker)))
                .Select(ticker => new SymbolPairForBinanceAndGateIo
                {
                    BinanceTicker = ticker,
                    GateIoTicker = ReplaceBinanceTickerToGateIo(ticker)
                })
                .ToList();

            foreach (var symbolPair in symbolPairs)
            {
                var priceBinanceTask = _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                var priceGateIoTask = _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);

                await Task.WhenAll(priceBinanceTask, priceGateIoTask);

                var priceBinance = priceBinanceTask.Result;
                var priceGateIo = priceGateIoTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceBinance, priceGateIo);
            }

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 1)
                {
                    var priceBinance = await _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                    var priceGateIo = await _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);

                    string additionalText = GetAdditionalText(symbolPair.GateIoTicker, tickersDiscribe);

                    Console.WriteLine($"{symbolPair.BinanceTicker}, Difference: {symbolPair.PercentDifference}, Binance: {priceBinance}, GateIo: {priceGateIo}{additionalText}");
                }
            }
        }

        private string GetAdditionalText(string gateIoTicker, string[] tickersDiscribe)
        {
            if (tickersDiscribe.Contains(gateIoTicker))
            {
                return " Not relevant";
            }

            return string.Empty;
        }

        private string ReplaceBinanceTickerToGateIo(string binanceTicker)
        {
            return binanceTicker.Replace("USDT", "_USDT")
                .Replace("USD", "_USD")
                .Replace("ETH", "_ETH")
                .Replace("TRY", "_TRY")
                .Replace("BTC", "_BTC")
                .Replace("BNB", "_BNB");
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
