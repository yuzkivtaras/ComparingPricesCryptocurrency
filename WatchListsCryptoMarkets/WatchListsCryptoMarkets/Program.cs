using CryptoExchange.Net.CommonObjects;
using WatchListsCryptoMarkets.ComparerPrice;
using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;


namespace WatchListsCryptoMarkets
{
    public class Program
    {
        public static async Task Main()
        {
            //Binance API
            var binanceTickerApiService = new BinanceTickerApiService(new HttpClient());
            var binancePriceApiService = new BinancePriceApiService(new HttpClient());

            //var tickersBinance = await binanceTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickersBinance)
            //{
            //    var symbol = ticker.ToString();
            //    var priceBinance = await binancePriceApiService.GetPriceAsync(symbol);
            //    Console.WriteLine($"Binance - Symbol: {symbol}, Price: {priceBinance}");
            //}

            //GateIo API
            //var gateIoTickerApiService = new GateIoTickerApiService(new HttpClient());
            //var gateIoPriceApiService = new GateIoPriceApiService(new HttpClient());

            //var tickersGateIo = await gateioTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickersGateIo)
            //{
            //    var symbol = ticker.ToString();
            //    var priceGateIo = await gateioPriceApiService.GetPriceAsync(symbol);
            //    Console.WriteLine($"GateIo - Symbol: {symbol}, Price: {priceGateIo}");
            //}

            //ByBit API
            var byBitTickerApiService = new ByBitTickerApiService(new HttpClient());
            var byBitPriceApiService = new ByBitPriceApiService(new HttpClient());

            //var tickeersByBit = await byBitTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickeersByBit)
            //{
            //    var symbol = ticker.ToString();
            //    var priceByBit = await byBitPriceApiService.GetPriceAsync(symbol);
            //    Console.WriteLine($"ByBit - Symbol: {symbol}, Price: {priceByBit}");
            //}

            //ComparePricesAsync

            //BinanceAndGateIo
            //var comparerBinanceAndGateIo = new BinanceAndGateIoComparerPrice(binanceTickerApiService, binancePriceApiService, gateIoTickerApiService, gateIoPriceApiService);
            //await comparerBinanceAndGateIo.ComparerPrice();

            //BiNanceAndByBit
            var comparerBinanceAndByBit = new BinanceAndByBitComparerPrice(binanceTickerApiService, binancePriceApiService, byBitTickerApiService, byBitPriceApiService);
            await comparerBinanceAndByBit.ComparerPrice();
        }
    }
}











