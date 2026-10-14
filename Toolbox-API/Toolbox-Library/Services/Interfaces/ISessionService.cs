using System;
using System.Collections.Generic;
using System.Text;
using Toolbox_Library.Utilities;

namespace Toolbox_Library.Services.Interfaces
{
    public interface ISessionService
    {
        /// <summary>
        /// Used to add a session key and the value and the TTL(Time to Live) data to the database
        /// </summary>
        /// <param name="key">The Session Key</param>
        /// <param name="sessionValue">The value which is a Json Serialized string of the data.</param>
        /// <param name="ttl">The Time to Live flag.</param>
        public Task<string> AddSessionToDB(string sessionValue);

        /// <summary>
        /// Retrieves the session value from a session key and deserializes the value back into a class
        /// </summary>
        /// <param name="key">The Session Key from the cookie.</param>
        /// <returns>A SessionData class</returns>
        public Task<SessionData> GetSessionDataFromKey(string key);

        /// <summary>
        /// Deletes a session from the database. Used in Logout functions
        /// </summary>
        /// <param name="sessionKey">The Session Key</param>
        /// <returns>True or False if the action was completed successfully</returns>
        public Task<bool> DeleteSessionFromDB(string sessionKey);

        /// <summary>
        /// Searches the values for matching UserId and deletes it. Used to ensure no session key is available for an unavailable account e.g Deleted User.
        /// </summary>
        /// <param name="inputValue">A Guid converted into a string</param>
        public Task DeleteAllSessionsFromUserId(string inputValue);

        /// <summary>
        /// Validates a session key to check if it still exists in the Redis database. Used for Authentication.
        /// </summary>
        /// <param name="sessionKey">The session Key</param>
        /// <returns>True or False whether the key still exists in the database or not.</returns>
        public Task<bool> ValidateSession(string sessionKey);
    }
}
