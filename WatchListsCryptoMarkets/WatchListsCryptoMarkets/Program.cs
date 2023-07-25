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
            //Stable Coins: USDT, TUSD, BUSD, USDC
            //Coins: BNB, BTC, ETH, DAI, VAI, XRP, TRX, DOGE, DOT
            //FIAT: TRY, EUR, BRL, ARS, BIDR, GBP, IDRT, NGN, PLN, RON, RUB, UAH, ZAR
            var binanceTickerApiService = new BinanceTickerApiService(new HttpClient());
            var binancePriceApiService = new BinancePriceApiService(new HttpClient());

            //var tickersBinance = await binanceTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickersBinance)
            //{
            //    var symbol = ticker.ToString();
            //    var priceBinance = await binancePriceApiService.GetPriceAsync(symbol);
            //    //Console.WriteLine($"Binance - Symbol: {symbol}, Price: {priceBinance}");
            //    Console.WriteLine(symbol);
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
            //    //var priceKucoin = await kucoinPriceApiService.GetPriceAsync(symbol);

            //    //Console.WriteLine($"Kucoin - Symbol: {symbol}, Price: {priceKucoin}");
            //}

            //Kraken API
            var krakenTickerApiService = new KrakenTicketApiService(new HttpClient());
            var krakenPriceApiService = new KrakenPriceApiService(new HttpClient());

            //var tickersKraken = await krakenTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickersKraken)
            //{
            //    var symbol = ticker.ToString();
            //    var priceKraken = await krakenPriceApiService.GetPriceAsync(symbol);

            //    Console.WriteLine($"Kraken - Symbol: {symbol}, Price: {priceKraken}");
            //}

            //OKX API
            var okxTickerApiService = new OkxTickerApiService(new HttpClient());
            var okxPriceApiService = new OkxPriceApiService(new HttpClient());

            //var tickersOkx = await okxTickerApiService.GetTickersAsync();
            //foreach (var ticker in tickersOkx)
            //{
            //    var symbol = ticker.ToString();
            //    var priceOkx = await okxPriceApiService.GetPriceAsync(symbol);

            //    Console.WriteLine($"OKX - Symbol: {symbol}, Price: {priceOkx}");
            //    //Console.WriteLine(symbol);
            //}

            var bitfinexTickerApiService = new BitfinexTickerApiService(new HttpClient());
            var bitfinexPriceApiService = new BitfinexPriceApiService(new HttpClient());

            var tickersBitfinex = await bitfinexTickerApiService.GetTickersAsync();
            foreach (var ticker in tickersBitfinex)
            {
                var symbol = ticker.ToString();
                var priceBitfinex = await bitfinexPriceApiService.GetPriceAsync(symbol);

                Console.WriteLine($"Bitfinex - Symbol: {symbol}, Price: {priceBitfinex}");
                //Console.WriteLine(symbol);
            }

            //ComparePricesAsync

            ////BinanceAndByBit
            //Console.WriteLine("-----------Binance - ByBit-----------");
            //var comparerBinanceAndByBit = new BinanceAndByBitComparerPrice(binanceTickerApiService, binancePriceApiService, byBitTickerApiService, byBitPriceApiService);
            //await comparerBinanceAndByBit.ComparerPrice();

            ////BinanceAndGateIo
            //Console.WriteLine("-----------Binance - GateIo-----------");
            //var comparerBinanceAndGateIo = new BinanceAndGateIoComparerPrice(binanceTickerApiService, binancePriceApiService, gateIoTickerApiService, gateIoPriceApiService);
            //await comparerBinanceAndGateIo.ComparerPrice();

            ////BinanceAndKraken
            //Console.WriteLine("-----------Binance - Kraken-----------");
            //var comparerBinanceAndKraken = new BinanceAndKrakenComparerPrice(binanceTickerApiService, binancePriceApiService, krakenTickerApiService, krakenPriceApiService);
            //await comparerBinanceAndKraken.ComparerPrice();

            ////BinanceAndKucoin
            //Console.WriteLine("-----------Binance - Kucoin-----------");
            //var comparerBinanceAndKucoin = new BinanceAndKucoinComparerPrice(binanceTickerApiService, binancePriceApiService, kucoinTickerApiService, kucoinPriceApiService);
            //await comparerBinanceAndKucoin.ComparerPrice();

            ////BinanceAndOKX
            //Console.WriteLine("-----------Binance - OKX-----------");
            //var comparerBinanceAndOkx = new BinanceAndOkxComparerPrice(binanceTickerApiService, binancePriceApiService, okxTickerApiService, okxPriceApiService);
            //await comparerBinanceAndOkx.ComparerPrice();

            ////ByBitAndGateIo
            //Console.WriteLine("-----------ByBit - GateIo-----------");
            //var comparerByBitAndGateIo = new ByBitAndGateIoComparerPrice(byBitTickerApiService, byBitPriceApiService, gateIoTickerApiService, gateIoPriceApiService);
            //await comparerByBitAndGateIo.ComparerPrice();

            ////ByBitAndKraken
            //Console.WriteLine("-----------ByBit - Kraken-----------");
            //var comparerByBitAndKraken = new ByBitAndKrakenComparerPrice(byBitTickerApiService, byBitPriceApiService, krakenTickerApiService, krakenPriceApiService);
            //await comparerByBitAndKraken.ComparerPrice();

            ////ByBitAndKucoin
            //Console.WriteLine("-----------ByBit - Kucoin-----------");
            //var comparerByBitAndKucoin = new ByBitAndKucoinComparerPrice(byBitTickerApiService, byBitPriceApiService, kucoinTickerApiService, kucoinPriceApiService);
            //await comparerByBitAndKucoin.ComparerPrice();

            ////ByBitAndOKX
            //Console.WriteLine("-----------ByBit - OKX-----------");
            //var comparerByBitandOkx = new ByBitAndOkxComparerPrice(byBitTickerApiService, byBitPriceApiService, okxTickerApiService, okxPriceApiService);
            //await comparerByBitandOkx.ComparerPrice();

            ////GateIoAndKraken
            //Console.WriteLine("-----------GateIo - Kraken-----------");
            //var comparerGateIoAndKraken = new GateIoAndKrakenComaparerPrice(gateIoTickerApiService, gateIoPriceApiService, krakenTickerApiService, krakenPriceApiService);
            //await comparerGateIoAndKraken.ComparerPrice();

            ////GateIoAndKucoin
            //Console.WriteLine("-----------GateIo - Kucoin-----------");
            //var comparerGateIoAndKucoin = new GateIoAndKucoinComparerPrice(gateIoTickerApiService, gateIoPriceApiService, kucoinTickerApiService, kucoinPriceApiService);
            //await comparerGateIoAndKucoin.ComparerPrice();

            ////GateIoAndOKX
            //Console.WriteLine("-----------GateIo - OKX-----------");
            //var comparerGateIoAndOkx = new GateIoAndOkxComparerPrice(gateIoTickerApiService, gateIoPriceApiService, okxTickerApiService, okxPriceApiService);
            //await comparerGateIoAndOkx.ComparerPrice();

            ////KrakenAndOKX
            //Console.WriteLine("-----------Kraken - OKX-----------");
            //var comparerKrakenAndOkx = new KrakenAndOkxComparerPrice(krakenTickerApiService, krakenPriceApiService, okxTickerApiService, okxPriceApiService);
            //await comparerKrakenAndOkx.ComparerPrice();

            ////KucoinAndKraken
            //Console.WriteLine("-----------Kucoin - Kraken-----------");
            //var comparerKucoinAndKraken = new KucoinAndKrakenComparerPrice(kucoinTickerApiService, kucoinPriceApiService, krakenTickerApiService, krakenPriceApiService);
            //await comparerKucoinAndKraken.ComparerPrice();

            ////KucoinAndOKX
            //Console.WriteLine("-----------Kucoin - OKX-----------");
            //var comparerKucoinAndOkx = new KucoinAndOkxComparerPrice(kucoinTickerApiService, kucoinPriceApiService, okxTickerApiService, okxPriceApiService);
            //await comparerKucoinAndOkx.ComparerPrice();
        }
    }
}











