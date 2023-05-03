using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CryptoExchange.Net.CommonObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

///////////////////////////////////////////////////////////////////////////////////

//Binance
//API Key
//u8B2CFeKPRGiVLNLK7J5QpjhgUFXZ4g2qRSyb3S9k7oqCPFKAPFs4gX5O6v6yAcb
//Secret Key
//LVnpPA9LoOgQQcpIKUOYmGEI16tsrv3K30rq6OLU9gRnWTeSN10IFBd25oB5u6xj

///////////////////////////////////////////////////////////////////////////////////

var clientBinance = new HttpClient();
var responseBinance = await clientBinance.GetAsync("https://api.binance.com/api/v3/ticker/price");

if (!responseBinance.IsSuccessStatusCode)
{
    Console.WriteLine($"Failed to get ticker info from Binance API: {responseBinance.ReasonPhrase}");
    return;
}

var jsonBinance = await responseBinance.Content.ReadAsStringAsync();
var tickerInfoBinance = JArray.Parse(jsonBinance);

var clientByBit = new HttpClient();
var responseByBit = await clientByBit.GetAsync("https://api.bybit.com/v2/public/tickers");

if (!responseByBit.IsSuccessStatusCode)
{
    Console.WriteLine($"Failed to get tickers from ByBit API: {responseByBit.ReasonPhrase}");
    return;
}

var jsonByBit = await responseByBit.Content.ReadAsStringAsync();
var tickerInfoByBitObj = JObject.Parse(jsonByBit);
var tickerInfoByBit = tickerInfoByBitObj["result"].ToObject<JArray>();

var commonSymbols = tickerInfoBinance.Select(t => t["symbol"].ToString())
                        .Intersect(tickerInfoByBit.Select(t => t["symbol"].ToString()));

var results = new List<(string symbol, decimal binancePrice, decimal byBitPrice, decimal priceDiff)>();

Console.WriteLine("------------Спільні торгові пари:------------");

foreach (var symbol in commonSymbols)
{
    var binancePrice = tickerInfoBinance.FirstOrDefault(t => t["symbol"].ToString() == symbol)["price"].ToObject<decimal>();
    var byBitPrice = tickerInfoByBit.FirstOrDefault(t => t["symbol"].ToString() == symbol)["last_price"].ToObject<decimal>();

    var priceDiff = binancePrice - byBitPrice;

    results.Add((symbol, binancePrice, byBitPrice, priceDiff));
}

foreach (var result in results.OrderByDescending(r => r.priceDiff))
{
    Console.WriteLine($"{result.symbol}: Binance: {result.binancePrice:F4} - ByBit: {result.byBitPrice:F4} = {result.priceDiff:F4}");
}







