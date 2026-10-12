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

        private const int NonceSize = 12; // GCM's IV
        private const int TagSize = 16; // Authentication tag
        private byte[] _EncryptionKey {  get; set; }

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

        public Task<string> Argon2HashString(string input)
        {
            throw new NotImplementedException();
        }
    }
}
