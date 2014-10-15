﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IF.Lastfm.Core.Api.Commands.AuthApi;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using xBrainLab.Security.Cryptography;

namespace IF.Lastfm.Core.Api
{
    public class LastAuth : ILastAuth
    {
        private readonly string _apiSecret;

        public bool Authenticated { get { return UserSession != null; } }
        public string ApiKey { get; private set; }
        public LastUserSession UserSession { get; private set; }

        public LastAuth(string apikey, string secret)
        {
            ApiKey = apikey;
            _apiSecret = secret;
        }

        /// <summary>
        /// Load an existing user session
        /// </summary>
        /// <param name="session">Session to load</param>
        /// <returns>Whether session object is valid</returns>
        public bool LoadSession(LastUserSession session)
        {
            UserSession = session;
            return true;
        }

        public async Task<LastResponse> GetSessionTokenAsync(string username, string password)
        {
            var command = new GetMobileSessionCommand(this, username, password);
            var response = await command.ExecuteAsync();

            if (response.Success)
            {
                UserSession = response.Content;
                return LastResponse.CreateSuccessResponse();
            }
            else
            {
                return LastResponse.CreateErrorResponse<LastResponse>(response.Error);
            }
        }

        public string GenerateMethodSignature(string method, Dictionary<string, string> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new Dictionary<string, string>();
            }

            parameters.Add("api_key", ApiKey);
            parameters.Add("method", method);

            var builder = new StringBuilder();

            foreach (var kv in parameters.OrderBy(kv => kv.Key))
            {
                builder.Append(kv.Key);
                builder.Append(kv.Value);
            }

            builder.Append(_apiSecret);

            return MD5.GetHashString(builder.ToString());
        }
    }
}