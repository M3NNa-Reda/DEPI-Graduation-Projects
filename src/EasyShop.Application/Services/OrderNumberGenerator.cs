using System.Security.Cryptography;

namespace EasyShop.Application.Services
{
    // Builds the public order reference (for example ES-7K3M9XQ2PD). It is random, so it says
    // nothing about how many orders the shop has and cannot be guessed from another one.
    public static class OrderNumberGenerator
    {
        private const string Prefix = "ES-";
        private const int RandomLength = 10;

        // No 0, 1, I, L, O so a number read out over the phone is not misheard.
        private const string Alphabet = "23456789ABCDEFGHJKMNPQRSTUVWXYZ";

        public static string Next()
        {
            Span<char> chars = stackalloc char[RandomLength];
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = Alphabet[RandomNumberGenerator.GetInt32(Alphabet.Length)];
            }

            return Prefix + new string(chars);
        }
    }
}
