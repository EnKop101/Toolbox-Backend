using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox_Library.Utilities.Encryption.Interface
{
    public interface IEncryptionHandler
    {
        public Task<string> DecryptString(string input);

        public Task<string> EncryptString(string input);

        public Task<string> Sha256HashString(string input);
        public Task<string> Argon2HashString(string input);
        public Task<bool> VerifyArgonHashedInput(string input, string storedHash);
    }
}
