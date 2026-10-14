using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Utilities;

namespace Toolbox_Library.Repositories.Interfaces
{

    public interface IRedisRepository
    {
        public Task<bool> AddSessionToDB(string key, string sessionValue, TimeSpan ttl);

        public Task<string> GetSessionDataFromKey(string key);

        public Task<bool> DeleteSessionFromDB(string sessionKey);

        public Task DeleteAllSessionsFromUserId(string inputValue);

        public Task<bool> ValidateSession(string sessionKey);
    }
}
