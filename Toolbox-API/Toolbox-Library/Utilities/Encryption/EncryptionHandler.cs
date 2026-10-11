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
        private string _EncryptionKey {  get; set; }

        public EncryptionHandler(string saltKey, string encryptionKey)
        {
            _SALT = saltKey;
            _EncryptionKey = encryptionKey;
        }

        public async Task<string> DecryptString(string input)
        {
            // Convert the Base64 encrypted string back into raw bytes
            byte[] fullCipher = Convert.FromBase64String(input);

            using (Aes aes = Aes.Create())
            {
                // Generate the AES key from the secret encryption key using SHA256
                using (var sha256 = SHA256.Create())
                {
                    byte[] keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_EncryptionKey));
                    aes.Key = keyBytes;
                }

                // Extract the Initialization Vector (IV) from the start of the encrypted data
                byte[] iv = new byte[aes.BlockSize / 8];
                byte[] cipher = new byte[fullCipher.Length - iv.Length];

                Array.Copy(fullCipher, iv, iv.Length);                 // Copy IV
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length); // Copy actual encrypted data

                // Assign the extracted IV to AES
                aes.IV = iv;

                // Create the decryptor using the AES key and IV
                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(cipher))              // Load encrypted data into memory
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read)) // Decrypt stream
                using (var sr = new StreamReader(cs))                  // Read decrypted data as text
                {
                    // Return the fully decrypted string
                    return await sr.ReadToEndAsync();
                }
            }
        }

        public async Task<string> EncryptString(string input)
        {
            // Create a new AES encryption instance
            using (Aes aes = Aes.Create())
            {
                // Derive a fixed encryption key from the secret key using SHA-256
                using (var sha256 = SHA256.Create())
                {
                    // Convert the secret key into a 256-bit hash to use as the AES key
                    byte[] keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(_EncryptionKey));
                    aes.Key = keyBytes;
                }

                // Generate a unique IV for this encryption operation
                aes.GenerateIV(); // random IV for each encryption

                // Create an encryptor using the derived key and generated IV
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream())
                {
                    // Prepend the IV to the encrypted output so it can be used during decryption
                    await ms.WriteAsync(aes.IV, 0, aes.IV.Length);

                    // Create a CryptoStream to handle the encryption process
                    using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

                    // Write the plaintext into the CryptoStream, which encrypts it
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(input);
                    }

                    // Convert the encrypted byte array (including IV) into a Base64 string and return it
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public Task<string> HashString(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                // Convert string to bytes
                byte[] bytes = Encoding.UTF8.GetBytes(_SALT + input + _SALT);

                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convert hash bytes to hex string
                StringBuilder sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("x2")); // lowercase hex

                return Task.FromResult(sb.ToString());
            }
        }
    }
}
