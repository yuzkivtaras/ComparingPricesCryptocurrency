using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class GateIoAndOkxComparerPrice
    {
        private readonly GateIoTickerApiService _gateIoTickerApiService;
        private readonly GateIoPriceApiService _gateIoPriceApiService;
        private readonly OkxTickerApiService _okxTickerApiService;
        private readonly OkxPriceApiService _okxPriceApiService;

        public GateIoAndOkxComparerPrice(GateIoTickerApiService gateIoTickerApiService, 
            GateIoPriceApiService gateIoPriceApiService, 
            OkxTickerApiService okxTickerApiService, 
            OkxPriceApiService okxPriceApiService)
        {
            _gateIoTickerApiService = gateIoTickerApiService;
            _gateIoPriceApiService = gateIoPriceApiService;
            _okxTickerApiService = okxTickerApiService;
            _okxPriceApiService = okxPriceApiService;
        }

        public async Task ComparerPrice()
        {
            string[] notAvailableDeposit = new string[] { "FLM_USDT" };

            var tickersGateIo = await _gateIoTickerApiService.GetTickersAsync();
            var tickersOkx = await _okxTickerApiService.GetTickersAsync();

            var symbolPairs = tickersGateIo
                .Where(ticker => tickersOkx.Contains(ReplaceGateIoTickerToOkx(ticker)) && !notAvailableDeposit.Contains(ticker))
                .Select(ticker => new SymbolPairForGateIoAndOkx
                {
                     GateIoTicker = ticker,
                     OkxTicker = ReplaceGateIoTickerToOkx(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceGateIoTask = _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);
                var priceOkxTask = _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                await Task.WhenAll(priceGateIoTask, priceOkxTask);

                var priceGateIo = priceGateIoTask.Result;
                var priceOkx = priceOkxTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceGateIo, priceOkx);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 1)
                {
                    var priceGateIo = await _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);
                    var priceOkx = await _okxPriceApiService.GetPriceAsync(symbolPair.OkxTicker);

                    Console.WriteLine($"{symbolPair.GateIoTicker}, Difference: {symbolPair.PercentDifference}, GateIo: {priceGateIo}, OKX: {priceOkx}");
                }
            }
        }

        private string ReplaceGateIoTickerToOkx(string okxTicker)
        {
            return okxTicker.Replace("_ETH", "-ETH-SWAP")
                .Replace("_BTC", "-BTC-SWAP")
                .Replace("_USDC", "-USDC-SWAP")
                .Replace("_USDT", "-USDT-SWAP");
        }

        private double CalculatePriceDifferencePercent(decimal priceGateIo, decimal priceOkx)
        {
            var priceDifference = Math.Abs(priceGateIo - priceOkx);
            return (double)priceDifference / (double)priceGateIo * 100;
        }
    }

    class SymbolPairForGateIoAndOkx
    {
        public string GateIoTicker { get; set; }
        public string OkxTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
