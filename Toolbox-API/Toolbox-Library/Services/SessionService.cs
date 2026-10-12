using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Toolbox_Library.Repositories.Interfaces;
using Toolbox_Library.Services.Interfaces;
using Toolbox_Library.Utilities;
using Toolbox_Library.Utilities.Encryption.Interface;

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

        public async Task<bool> AddSessionToDB(string sessionValue)
        {
            string rawKey = SessionIdGenerator();
            string encryptedValue = await _encryptionHandler.EncryptString(sessionValue);
            TimeSpan ttl = TimeSpan.FromDays(3);
            return await _redisRepository.AddSessionToDB(rawKey, encryptedValue, ttl);
        }

        public Task DeleteAllSessionsFromUserId(string inputValue)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSessionFromDB(string sessionKey)
        {
            throw new NotImplementedException();
        }

        public Task<SessionData> GetSessionDataFromKey(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidateSession(string sessionKey)
        {
            throw new NotImplementedException();
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
