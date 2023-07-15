using CryptoExchange.Net.CommonObjects;
using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class ByBitAndGateIoComparerPrice
    {
        private readonly ByBitTickerApiService _byBitTickerApiService;
        private readonly ByBitPriceApiService _byBitPriceApiService;
        private readonly GateIoTickerApiService _gateIoTickerApiService;
        private readonly GateIoPriceApiService _gateIoPriceApiService;

        public ByBitAndGateIoComparerPrice(ByBitTickerApiService byBitTickerApiService, 
            ByBitPriceApiService byBitPriceApiService, 
            GateIoTickerApiService gateIoTickerApiService, 
            GateIoPriceApiService gateIoPriceApiService)
        {
            _byBitTickerApiService = byBitTickerApiService;
            _byBitPriceApiService = byBitPriceApiService;
            _gateIoTickerApiService = gateIoTickerApiService;
            _gateIoPriceApiService = gateIoPriceApiService;
        }

        public async Task ComparerPrice()
        {
            string[] differentСurrency = new string[] { "ORTUSDT", "AXLUSDT", "REALUSDT", "TIMEUSDT", "FAMEUSDT" };
            string[] bigRent = new string[] { "SAITAMAUSDT", "KOKUSDT", "AGLAUSDT" };

            var tickersByBit = await _byBitTickerApiService.GetTickersAsync();
            var tickersGateIo = await _gateIoTickerApiService.GetTickersAsync();
            var symbolPairs = tickersByBit
                .Where(ticker => tickersGateIo.Contains(ReplaceByBitTickerToGateIo(ticker)) && !differentСurrency.Contains(ticker) && !bigRent.Contains(ticker))
                .Select(ticker => new SymbolPairForByBitAndGateIo
                {
                    ByBitTicker = ticker,
                    GateIoTicker = ReplaceByBitTickerToGateIo(ticker)
                }) 
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceByBitTask = _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);
                var priceGateIoTask = _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);

                await Task.WhenAll(priceByBitTask, priceGateIoTask);

                var priceByBit = priceByBitTask.Result;
                var priceGateIo = priceGateIoTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceByBit, priceGateIo);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 4)
                {
                    var priceByBit = await _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);
                    var priceGateIo = await _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);

                    Console.WriteLine($"{symbolPair.ByBitTicker}, Difference: {symbolPair.PercentDifference}, ByBit: {priceByBit}, GateIo: {priceGateIo}");
                }
            }
        }

        private string ReplaceByBitTickerToGateIo(string ByBitTicker)
        {
            return ByBitTicker.Replace("USDT", "_USDT")
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

        private double CalculatePriceDifferencePercent(decimal priceByBit, decimal priceGateIo)
        {
            var priceDifference = Math.Abs(priceByBit - priceGateIo);
            return (double)priceDifference / (double)priceByBit * 100;
        }
    }

    class SymbolPairForByBitAndGateIo
    {
        public string ByBitTicker { get; set; }
        public string GateIoTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
