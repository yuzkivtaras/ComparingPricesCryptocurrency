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
                var priceBinance = await _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                var priceKucoin = await _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceBinance, priceKucoin);                          
            }

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                var priceBinance = await _binancePriceApiService.GetPriceAsync(symbolPair.BinanceTicker);
                var priceGateIo = await _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);

                if (symbolPair.PercentDifference >= 0.1)
                {
                    Console.WriteLine($"{symbolPair.BinanceTicker}, Difference: {symbolPair.PercentDifference}, Binance: {priceBinance}, GateIo: {priceGateIo}");
                }
            }           
        }

        private string ReplaceBinanceTickerToGateIo(string binanceTicker)
        {
            return binanceTicker.Replace("USDT", "_USDT");
            //.Replace("ETH", "_ETH")
            //.Replace("USD", "_USD")
            //.Replace("BTC", "_BTC")
            //.Replace("BNB", "_BNB");
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
