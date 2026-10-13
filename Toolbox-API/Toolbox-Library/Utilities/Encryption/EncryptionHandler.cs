using Konscious.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Toolbox_Library.Utilities.Encryption.Interface;

namespace Toolbox_Library.Utilities.Encryption
{
    public class EncryptionHandler : IEncryptionHandler
    {
        private string _SALT {  get; set; }
        private const int _memorySizeKb = 65536;
        private const int _iterations = 7;
        private const int _parallelism = 4;
        private const int _hashLength = 32; // Bytes
        private const int _saltLength = 16;

        #region Encryption Properties
        private const int NonceSize = 12; // GCM's IV
        private const int TagSize = 16; // Authentication tag
        private byte[] _EncryptionKey {  get; set; }
        #endregion
        public EncryptionHandler(string saltKey, byte[] keyBytes)
        {
            _SALT = saltKey;

            try
            {
                // Hash the plaintext string and ensures its exactly 32 bytes.
                _EncryptionKey = SHA256.HashData(keyBytes);
            }
            finally
            {
                // Clear the plaintext key string bytes from memory
                CryptographicOperations.ZeroMemory(keyBytes);
            }
        }

        public async Task<string> EncryptString(string input)
        {
            byte[] plain = Encoding.UTF8.GetBytes(input);
            byte[] output = new byte[NonceSize + plain.Length + TagSize];

            Span<byte> nonce = output.AsSpan(0, NonceSize);
            Span<byte> cipher = output.AsSpan(NonceSize, plain.Length);
            Span<byte> tag = output.AsSpan(NonceSize + plain.Length, TagSize);

            // Random nonce for each encryption (same role as GenerateIV)
            RandomNumberGenerator.Fill(nonce);

            using var aes = new AesGcm(_EncryptionKey, TagSize);
            aes.Encrypt(nonce, plain, cipher, tag);

            // Stored layout: [nonce | ciphertext | tag]
            return Convert.ToBase64String(output);
        }

        public async Task<string> DecryptString(string encryptedInput)
        {
            byte[] input = Convert.FromBase64String(encryptedInput);
            if (input.Length < NonceSize + TagSize)
                throw new CryptographicException("Invalid encrypted data.");

            int cipherLen = input.Length - NonceSize - TagSize;
            ReadOnlySpan<byte> nonce = input.AsSpan(0, NonceSize);
            ReadOnlySpan<byte> cipher = input.AsSpan(NonceSize, cipherLen);
            ReadOnlySpan<byte> tag = input.AsSpan(NonceSize + cipherLen, TagSize);

            byte[] plain = new byte[cipherLen];
            using var aes = new AesGcm(_EncryptionKey, TagSize);

            // Throws AuthenticationTagMismatchException if the data was
            // tampered with or the wrong key is used
            aes.Decrypt(nonce, cipher, tag, plain);

            return Encoding.UTF8.GetString(plain);
        }

        public Task<string> Sha256HashString(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                // Convert string to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(input + _SALT);

                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convert hash bytes to hex string
                StringBuilder sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("x2")); // lowercase hex

                return Task.FromResult(sb.ToString());
            }
        }

        public async Task<string> Argon2HashString(string input)
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(_saltLength);
            if (saltBytes.Length < 8) throw new ArgumentException("Salt must be at least 8 bytes");

            byte[] hash = await ComputeHash(input, saltBytes, _memorySizeKb, _iterations, _parallelism, _hashLength);

            return $"$argon2id$v=19$m={_memorySizeKb},t={_iterations},p={_parallelism}" +
                    $"${Convert.ToBase64String(saltBytes)}${Convert.ToBase64String(hash)}";

        }

        public async Task<bool> VerifyArgonHashedInput(string input, string storedHash)
        {
            // Expected parts: "", "argon2id", "v=19", "m=..,t=..,p=..", salt, hash
            string[] parts = storedHash.Split('$');
            if (parts.Length != 6 || parts[1] != "argon2id") return false;

            int m = 0, t = 0, p = 0;
            foreach (string kv in parts[3].Split(','))
            {
                string[] pair = kv.Split('=');
                int value = int.Parse(pair[1]);
                switch (pair[0])
                {
                    case "m": m = value; break;
                    case "t": t = value; break;
                    case "p": p = value; break;
                }
            }

            byte[] saltBytes = Convert.FromBase64String(parts[4]);
            byte[] expected = Convert.FromBase64String(parts[5]);
            byte[] actual = await ComputeHash(input, saltBytes, m, t, p, expected.Length);

            // Constant-time comparison to avoid timing attacks.
            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }


        private static async Task<byte[]> ComputeHash(string password, byte[] salt, int memoryKb, int iterations, int parallelism, int length)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                MemorySize = memoryKb,
                Iterations = iterations,
                DegreeOfParallelism = parallelism,
            };
            return await argon2.GetBytesAsync(length);
        }
    }
}
