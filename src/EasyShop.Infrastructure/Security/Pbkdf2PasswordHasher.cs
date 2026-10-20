using System.Security.Cryptography;
using EasyShop.Application.Interfaces.Services;

namespace EasyShop.Infrastructure.Security
{
    /// <summary>
    /// Hashes passwords with PBKDF2 (HMAC-SHA256) and a random salt per password.
    /// Stored format: {iterations}.{salt}.{key}, so the cost can be raised later
    /// without invalidating existing hashes.
    /// </summary>
    public class Pbkdf2PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int Iterations = 600_000;
        private const char Delimiter = '.';

        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
        private static readonly byte[] DummySalt = new byte[SaltSize];

        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

            return string.Join(Delimiter, Iterations, Convert.ToBase64String(salt), Convert.ToBase64String(key));
        }

        public bool Verify(string password, string? passwordHash)
        {
            if (!TryParse(passwordHash, out int iterations, out byte[] salt, out byte[] expectedKey))
            {
                // Do the same amount of work as a real check so a missing or malformed hash is not detectable by timing.
                Rfc2898DeriveBytes.Pbkdf2(password, DummySalt, Iterations, Algorithm, KeySize);
                return false;
            }

            byte[] actualKey = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, Algorithm, expectedKey.Length);
            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }

        private static bool TryParse(string? passwordHash, out int iterations, out byte[] salt, out byte[] key)
        {
            iterations = 0;
            salt = Array.Empty<byte>();
            key = Array.Empty<byte>();

            string[] parts = (passwordHash ?? string.Empty).Split(Delimiter);
            if (parts.Length != 3 || !int.TryParse(parts[0], out iterations) || iterations <= 0)
            {
                return false;
            }

            try
            {
                salt = Convert.FromBase64String(parts[1]);
                key = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            return salt.Length > 0 && key.Length > 0;
        }
    }
}
