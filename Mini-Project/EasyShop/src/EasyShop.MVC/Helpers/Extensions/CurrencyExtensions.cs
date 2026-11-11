using System.Globalization;

namespace EasyShop.MVC.Helpers.Extensions
{
    public static class CurrencyExtensions
    {
        private const string CurrencyCode = "EGP";

        // Always formats with the invariant culture so the output does not depend on the server's regional settings.
        public static string ToEgp(this decimal amount)
        {
            return $"{amount.ToString("N2", CultureInfo.InvariantCulture)} {CurrencyCode}";
        }

        public static string ToEgp(this decimal? amount)
        {
            return amount.HasValue ? amount.Value.ToEgp() : "-";
        }
    }
}
