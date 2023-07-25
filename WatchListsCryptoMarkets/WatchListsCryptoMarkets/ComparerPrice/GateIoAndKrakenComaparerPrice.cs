using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class GateIoAndKrakenComaparerPrice
    {
        private readonly GateIoTickerApiService _gateIoTickerApiService;
        private readonly GateIoPriceApiService _gateIoPriceApiService;
        private readonly KrakenTicketApiService _krakenTicketApiService;
        private readonly KrakenPriceApiService _krakenPriceApiService;

        public GateIoAndKrakenComaparerPrice(GateIoTickerApiService gateIoTickerApiService, 
            GateIoPriceApiService gateIoPriceApiService, 
            KrakenTicketApiService krakenTicketApiService, 
            KrakenPriceApiService krakenPriceApiService)
        {
            _gateIoTickerApiService = gateIoTickerApiService;
            _gateIoPriceApiService = gateIoPriceApiService;
            _krakenTicketApiService = krakenTicketApiService;
            _krakenPriceApiService = krakenPriceApiService;
        }

        public async Task ComparerPrice()
        {
            string[] unavailableOutputGateIo = new string[] { "MLN_ETH" };

            var tickersGateIo = await _gateIoTickerApiService.GetTickersAsync();
            var tickersKraken = await _krakenTicketApiService.GetTickersAsync();

            var symbolPairs = tickersGateIo
                .Where(ticker => tickersKraken.Contains(ReplaceGateIoTickerToKraken(ticker)) && !unavailableOutputGateIo.Contains(ticker))
                .Select(ticker => new SymbolPairForGateIoAndKraken
                {
                    GateIoTicker = ticker,
                    KrakenTicker = ReplaceGateIoTickerToKraken(ticker),
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceGateIoTask = _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);
                var priceKrakenTask = _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);

                await Task.WhenAll(priceGateIoTask, priceKrakenTask);

                var priceGateIo = priceGateIoTask.Result;
                var priceKraken = priceKrakenTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceGateIo, priceKraken);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 5)
                {
                    var priceGateIo = await _gateIoPriceApiService.GetPriceAsync(symbolPair.GateIoTicker);
                    var priceKraken = await _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);

                    Console.WriteLine($"{symbolPair.GateIoTicker}, Difference: {symbolPair.PercentDifference}, GateIo: {priceGateIo}, Kraken: {priceKraken}");
                }
            }
        }

        private string ReplaceGateIoTickerToKraken(string gateIoTicker)
        {
            return gateIoTicker.Replace("_USDT", "/USDT")
                .Replace("_TUSD", "/TUSD")
                .Replace("_BUSD", "/BUSD")
                .Replace("_USDC", "/USDC")
                .Replace("_BNB", "/BNB")
                .Replace("_BTC", "/BTC")
                .Replace("_ETH", "/ETH")
                .Replace("_DAI", "/DAI")
                .Replace("_VAI", "/VAI")
                .Replace("_XRP", "/XRP")
                .Replace("_TRX", "/TRX")
                .Replace("_DOGE", "/DOGE")
                .Replace("_DOT", "/DOT")
                .Replace("_TRY", "/TRY")
                .Replace("_EUR", "/EUR")
                .Replace("_BRL", "/BRL")
                .Replace("_ARS", "/ARS")
                .Replace("_BIDR", "/BIDR")
                .Replace("_GBP", "/GBP")
                .Replace("_IDRT", "/IDRT")
                .Replace("_NGN", "/NGN")
                .Replace("_PLN", "/PLN")
                .Replace("_RUB", "/RUB")
                .Replace("_UAH", "/UAH")
                .Replace("_ZAR", "/ZAR");
        }

        private double CalculatePriceDifferencePercent(decimal priceGateIo, decimal priceKraken)
        {
            var priceDifference = Math.Abs(priceGateIo - priceKraken);
            return (double)priceDifference / (double)priceGateIo * 100;
        }
    }

    class SymbolPairForGateIoAndKraken
    {
        public string GateIoTicker { get; set; }
        public string KrakenTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
