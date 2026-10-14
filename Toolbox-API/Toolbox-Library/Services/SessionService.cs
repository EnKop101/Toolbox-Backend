using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Toolbox_Library.Repositories.Interfaces;
using Toolbox_Library.Services.Interfaces;
using Toolbox_Library.Utilities;
using Toolbox_Library.Utilities.Encryption.Interface;
using static System.Collections.Specialized.BitVector32;
using static Toolbox_Library.Utilities.Response.HttpResponseException;

namespace Toolbox_Library.Services
{
    public class SessionService : ISessionService
    {
        private readonly IRedisRepository _redisRepository;
        private readonly IEncryptionHandler _encryptionHandler;

        public SessionService(IRedisRepository redisRepository, IEncryptionHandler encryptionHandler)
        {
            _redisRepository = redisRepository;
            _encryptionHandler = encryptionHandler;
        }

        public async Task<string> AddSessionToDB(string sessionValue)
        {
            bool duplicateKey = true;
            string key = "";
            do
            {
                string sessionKey = SessionIdGenerator();
                key = $"session:{sessionKey}";
                duplicateKey = await ValidateSession(key);
            } while (duplicateKey);

            string encryptedValue = await _encryptionHandler.EncryptString(sessionValue);
            TimeSpan ttl = TimeSpan.FromDays(3);
            if (await _redisRepository.AddSessionToDB(key, encryptedValue, ttl)) return key;
            else throw new InternalErrorException("Something went wrong when trying to create session");
        }

        public Task DeleteAllSessionsFromUserId(string inputValue)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteSessionFromDB(string sessionKey)
        {
            return await _redisRepository.DeleteSessionFromDB(sessionKey);
        }

        public async Task<SessionData> GetSessionDataFromKey(string key)
        {
            string encryptedSessionValue = await _redisRepository.GetSessionDataFromKey(key);
            string unencryptedSessionValue = await _encryptionHandler.DecryptString(encryptedSessionValue);

            SessionData sessionData = JsonSerializer.Deserialize<SessionData>(unencryptedSessionValue)
                ?? throw new NoContentException("Session key exists but could not deserialize value!");

            return sessionData;
        }

        public async Task<bool> ValidateSession(string sessionKey)
        {
            return await _redisRepository.ValidateSession(sessionKey);
        }

        private string SessionIdGenerator()
        {
            var bytes = new byte[32];
            RandomNumberGenerator.Fill(bytes);
            return Convert.ToBase64String(bytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .TrimEnd('=');
        }
    }
}
