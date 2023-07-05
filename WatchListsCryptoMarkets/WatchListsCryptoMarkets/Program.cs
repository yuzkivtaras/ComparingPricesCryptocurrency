﻿using CryptoExchange.Net.CommonObjects;
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
            var gateIoTickerApiService = new GateIoTickerApiService(new HttpClient());
            var gateIoPriceApiService = new GateIoPriceApiService(new HttpClient());

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

            //Kucoin API
            var kucoinTickerApiService = new KucoinTickerApiService(new HttpClient());
            var kucoinPriceApiService = new KucoinPriceApiService(new HttpClient());

            //var tickersKucoin = await kucoinTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickersKucoin)
            //{
            //    var symbol = ticker.ToString();
            //    var priceKucoin = await kucoinPriceApiService.GetPriceAsync(symbol);

            //    Console.WriteLine($"Kucoin - Symbol: {symbol}, Price: {priceKucoin}");
            //}

            //ComparePricesAsync

            ////BinanceAndGateIo
            //Console.WriteLine("-----------Binance - GateIo-----------");
            //var comparerBinanceAndGateIo = new BinanceAndGateIoComparerPrice(binanceTickerApiService, binancePriceApiService, gateIoTickerApiService, gateIoPriceApiService);
            //await comparerBinanceAndGateIo.ComparerPrice();

            ////BinanceAndByBit
            //Console.WriteLine("-----------Binance - ByBit-----------");
            //var comparerBinanceAndByBit = new BinanceAndByBitComparerPrice(binanceTickerApiService, binancePriceApiService, byBitTickerApiService, byBitPriceApiService);
            //await comparerBinanceAndByBit.ComparerPrice();

            ////ByBitAndGateIo
            //Console.WriteLine("-----------ByBit - GateIo-----------");
            //var comparerByBitAndGateIo = new ByBitAndGateIoComparerPrice(byBitTickerApiService, byBitPriceApiService, gateIoTickerApiService, gateIoPriceApiService);
            //await comparerByBitAndGateIo.ComparerPrice();

            //BinanceAndKucoin
            Console.WriteLine("-----------Binance - Kucoin-----------");
            var comparerBinanceAndKucoin = new BinanceAndKucoinComparerPrice(binanceTickerApiService, binancePriceApiService, kucoinTickerApiService, kucoinPriceApiService);
            await comparerBinanceAndKucoin.ComparerPrice();
        }
    }
}











