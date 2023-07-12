﻿using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

namespace WatchListsCryptoMarkets.ComparerPrice
{
    public class ByBitAndKrakenComparerPrice
    {
        private readonly ByBitTickerApiService _byBitTickerApiService;
        private readonly ByBitPriceApiService _byBitPriceApiService;
        private readonly KrakenTicketApiService _krakenTicketApiService;
        private readonly KrakenPriceApiService _krakenPriceApiService;

        public ByBitAndKrakenComparerPrice(ByBitTickerApiService byBitTickerApiService, 
            ByBitPriceApiService byBitPriceApiService, 
            KrakenTicketApiService krakenTicketApiService, 
            KrakenPriceApiService krakenPriceApiService)
        {
            _byBitTickerApiService = byBitTickerApiService;
            _byBitPriceApiService = byBitPriceApiService;
            _krakenTicketApiService = krakenTicketApiService;
            _krakenPriceApiService = krakenPriceApiService;
        }

        public async Task ComparerPrice()
        {
            var tickersByBit = await _byBitTickerApiService.GetTickersAsync();
            var tickersKraken = await _krakenTicketApiService.GetTickersAsync();

            var symbolPairs = tickersByBit
                .Where(ticker => tickersKraken.Contains(ReplaceByBitTickerToKraken(ticker)))
                .Select(ticker => new SymbolPairForByBitAndKraken
                {
                    ByBitTicker = ticker,
                    KrakenTicker = ReplaceByBitTickerToKraken(ticker)
                })
                .ToList();

            var tasks = symbolPairs.Select(async (symbolPair) =>
            {
                var priceByBitTask = _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);
                var priceKrakenTask = _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);

                await Task.WhenAll(priceByBitTask, priceKrakenTask);

                var priceByBit = priceByBitTask.Result;
                var priceKraken = priceKrakenTask.Result;

                symbolPair.PercentDifference = CalculatePriceDifferencePercent(priceByBit, priceKraken);
            });

            await Task.WhenAll(tasks);

            symbolPairs = symbolPairs.OrderByDescending(pair => pair.PercentDifference).ToList();

            foreach (var symbolPair in symbolPairs)
            {
                if (symbolPair.PercentDifference >= 0.5)
                {
                    var priceByBit = await _byBitPriceApiService.GetPriceAsync(symbolPair.ByBitTicker);
                    var priceKraken = await _krakenPriceApiService.GetPriceAsync(symbolPair.KrakenTicker);

                    Console.WriteLine($"{symbolPair.ByBitTicker}, Difference: {symbolPair.PercentDifference}, ByBit: {priceByBit}, Kraken: {priceKraken}");
                }
            }
        }

        private string ReplaceByBitTickerToKraken(string ByBitTicker)
        {
            return ByBitTicker.Replace("EUR", "/EUR")
                //.Replace("USD", "/USD")
                .Replace("ETH", "/ETH")
                .Replace("GBP", "/GBP")
                .Replace("XBT", "/XBT")
                .Replace("AUD", "/AUD")
                .Replace("JPY", "/JPY")
                .Replace("CAD", "/CAD")
                .Replace("USDC", "/USDC")
                .Replace("USDT", "/USDT");
        }

        private double CalculatePriceDifferencePercent(decimal priceByBit, decimal priceKraken)
        {
            var priceDifference = Math.Abs(priceByBit - priceKraken);
            return (double)priceDifference / (double)priceByBit * 100;
        }
    }

    class SymbolPairForByBitAndKraken
    {
        public string ByBitTicker { get; set; }
        public string KrakenTicker { get; set; }
        public double PercentDifference { get; set; }
    }
}
