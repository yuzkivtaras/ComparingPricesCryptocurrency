using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoExchange.Net.CommonObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        }
    }
}











