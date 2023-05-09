using Binance.Net.Objects.Models.Spot;
using System.Diagnostics;
using WatchListsCryptoMarkets.Services;
using WatchListsCryptoMarkets.Services.PriceApiService;
using WatchListsCryptoMarkets.Services.TickerApiService;

///////////////////////////////////////////////////////////////////////////////////

//Binance
//API Key
//u8B2CFeKPRGiVLNLK7J5QpjhgUFXZ4g2qRSyb3S9k7oqCPFKAPFs4gX5O6v6yAcb
//Secret Key
//LVnpPA9LoOgQQcpIKUOYmGEI16tsrv3K30rq6OLU9gRnWTeSN10IFBd25oB5u6xj

///////////////////////////////////////////////////////////////////////////////////


namespace WatchListsCryptoMarkets
{
    public class Program
    {
        public static async Task Main()
        {
            //Binance API
            //var binanceTickerApiService = new BinanceTickerApiService(new HttpClient());
            //var binancePriceApiService = new BinancePriceApiService(new HttpClient());

            //var tickersBinance = await binanceTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickersBinance)
            //{
            //    var symbol = ticker.ToString();
            //    var priceBinance = await binancePriceApiService.GetPriceAsync(symbol);
            //    Console.WriteLine($"Binance - Symbol: {symbol}, Price: {priceBinance}");
            //}

            //GateIo API
            //var gateioTickerApiService = new GateIoTickerApiService(new HttpClient());
            //var gateioPriceApiService = new GateIoPriceApiService(new HttpClient());

            //var tickersGateIo = await gateioTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickersGateIo)
            //{
            //    var symbol = ticker.ToString();
            //    var priceGateIo = await gateioPriceApiService.GetPriceAsync(symbol);
            //    Console.WriteLine($"GateIo - Symbol: {symbol}, Price: {priceGateIo}");
            //}

            //ComparePricesAsync
            var binanceTickerApiService = new BinanceTickerApiService(new HttpClient());
            var binancePriceApiService = new BinancePriceApiService(new HttpClient());

            var gateioTickerApiService = new GateIoTickerApiService(new HttpClient());
            var gateioPriceApiService = new GateIoPriceApiService(new HttpClient());

            var tickersBinance = await binanceTickerApiService.GetTickersAsync();
            var tickersGateIo = await gateioTickerApiService.GetTickersAsync();

            var symbolPairs = tickersBinance.Where(ticker => tickersGateIo.Contains(ReplaceBinanceTickerToGateIo(ticker)))
                                            .Select(ticker => (ticker, ReplaceBinanceTickerToGateIo(ticker)));

            int deley = 0;

            foreach (var symbolPair in symbolPairs)
            {
                var priceBinance = await binancePriceApiService.GetPriceAsync(symbolPair.Item1);
                var priceGateIo = await gateioPriceApiService.GetPriceAsync(symbolPair.Item2);

                var priceDifference = Math.Abs(priceBinance - priceGateIo);
                var priceDifferencePercent = priceDifference / priceBinance * 100;

                Console.WriteLine($"{symbolPair.Item1} Binance: {priceBinance}, Gate.Io: {priceGateIo}, Persent: {priceDifferencePercent}, Difference: {priceDifference}");    

                deley++;

                if (deley % 5 == 0)
                {
                    Thread.Sleep(1000);
                }
                
            }

            string ReplaceBinanceTickerToGateIo(string binanceTicker)
            {
                return binanceTicker.Replace("USDT", "_USDT")
                    .Replace("ETH", "_ETH")
                    .Replace("TRY", "_TRY")
                    .Replace("USD", "_USD")
                    .Replace("BTC", "_BTC");
            }
        }
    }
}











