using System;
using System.Security.Cryptography;

namespace akbars.Services
{
    public class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100000;

        public string Hash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return string.Empty;
            }

            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
            {
                var hash = deriveBytes.GetBytes(HashSize);
                return string.Format(
                    "pbkdf2${0}${1}${2}",
                    Iterations,
                    Convert.ToBase64String(salt),
                    Convert.ToBase64String(hash));
            }
        }

        public bool Verify(string password, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            if (!storedHash.StartsWith("pbkdf2$", StringComparison.OrdinalIgnoreCase))
            {
                return string.Equals(password, storedHash, StringComparison.Ordinal);
            }

            var parts = storedHash.Split('$');
            if (parts.Length != 4)
            {
                return false;
            }

            int iterations;
            if (!int.TryParse(parts[1], out iterations))
            {
                return false;
            }

            var salt = Convert.FromBase64String(parts[2]);
            var expectedHash = Convert.FromBase64String(parts[3]);

            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                var actualHash = deriveBytes.GetBytes(expectedHash.Length);
                return FixedTimeEquals(expectedHash, actualHash);
            }
        }

        private bool FixedTimeEquals(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length)
            {
                return false;
            }

            var diff = 0;
            for (var i = 0; i < left.Length; i++)
            {
                diff |= left[i] ^ right[i];
            }

            return diff == 0;
        }
    }
}
