using WatchListsCryptoMarkets.Services;

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
            var binanceApiService = new BinanceApiService(new HttpClient());
            var byBitApiService = new ByBitApiService(new HttpClient());

            var tickerComparer = new TickerComparer(binanceApiService, byBitApiService);
            var results = await tickerComparer.GetCommonSymbolsAsync();

            foreach (var result in results)
            {
                Console.WriteLine($"{result.symbol}: Binance: {result.binancePrice} - ByBit: {result.byBitPrice} = {result.priceDiff}");
            }

            var gateioApiService = new GateioApiService(new HttpClient());

            var tradingPairs = await gateioApiService.GetTickerInfoAsync();
            var topTradingPairs = tradingPairs.Select(p => (string)p["id"]).ToList();

            Console.WriteLine("------------------GateIo------------------");

            int batchSize = 70;
            for (int i = 0; i < topTradingPairs.Count; i += batchSize)
            {
                var batch = topTradingPairs.Skip(i).Take(batchSize).ToList();
                var tickerPrices = await gateioApiService.GetTickerPricesAsync(batch);

                foreach (var ticker in tickerPrices)
                {
                    var tradingPair = (string)ticker[0]["currency_pair"];
                    var lastPrice = (decimal)ticker[0]["last"];

                    Console.WriteLine($"Trading pair: {tradingPair}, last price: {lastPrice}");
                }

                if (i + batchSize < topTradingPairs.Count)
                {
                    Console.WriteLine("Waiting for 1 seconds before next batch...");
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}











