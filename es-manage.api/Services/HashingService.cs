using System;
using System.Security.Cryptography;
using System.Text;

namespace es_manage.api.Services
{
    public class HashingService
    {
        private const int KeySize = 64; // 64 bytes = 512 bits untuk SHA512
        private const int Iterations = 100000; // 100.000 iterasi (rekomendasi production)

        // Generate password yang di hash dan salt nya sebagai output
        public string HashPassword(string password, out string salt)
        {
            byte[] saltBytes = GenerateSalt(KeySize);
            byte[] hashBytes = GenerateHash(Encoding.UTF8.GetBytes(password), saltBytes, Iterations, KeySize);

            salt = Convert.ToBase64String(saltBytes);
            return Convert.ToBase64String(hashBytes);
        }

        // Verifikasi apakah password yang diinputkan sama dengan hash yang tersimpan di database
        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            byte[] hashBytes = Convert.FromBase64String(storedHash);
            byte[] passwordHashBytes = GenerateHash(Encoding.UTF8.GetBytes(password), saltBytes, Iterations, KeySize);

            return CryptographicOperations.FixedTimeEquals(passwordHashBytes, hashBytes);
        }

        // Method untuk generate hash
        private static byte[] GenerateHash(byte[] password, byte[] salt, int iterations, int keySize)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA512))
            {
                return rfc2898.GetBytes(keySize);
            }
        }

        // Method untuk generate salt
        private static byte[] GenerateSalt(int size)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var saltBytes = new byte[size];
                rng.GetBytes(saltBytes);
                return saltBytes;
            }
        }
    }
}