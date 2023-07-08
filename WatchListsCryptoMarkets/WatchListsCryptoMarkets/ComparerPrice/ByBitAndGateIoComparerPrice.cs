using CryptoExchange.Net.CommonObjects;
using System.Linq;
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
            string[] tickersDiscribe = new string[] { };

            var tickersByBit = await _byBitTickerApiService.GetTickersAsync();
            var tickersGateIo = await _gateIoTickerApiService.GetTickersAsync();

            var symbolPairs = tickersByBit
                .Where(ticker => tickersGateIo.Contains(ReplaceByBitTickerToGateIo(ticker)))
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

                    //string additionalText = GetAdditionalText(symbolPair.GateIoTicker, tickersDiscribe);

                    Console.WriteLine($"{symbolPair.ByBitTicker}, Difference: {symbolPair.PercentDifference}, ByBit: {priceByBit}, GateIo: {priceGateIo}");
                }
            }
        }

        private string ReplaceByBitTickerToGateIo(string ByBitTicker)
        {
            return ByBitTicker.Replace("ETH", "_ETH")
                //.Replace("DAI", "_DAI")
                .Replace("BTC", "_BTC")
                //.Replace("BIT", "_BIT");
                .Replace("USDT", "_USDT")
                .Replace("USDC", "_USDC");
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
