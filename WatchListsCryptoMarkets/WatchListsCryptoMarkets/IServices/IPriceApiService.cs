
namespace WatchListsCryptoMarkets.IServices
{
    public interface IPriceApiService
    {
        Task<decimal> GetPriceAsync(string symbol);
    }
}
