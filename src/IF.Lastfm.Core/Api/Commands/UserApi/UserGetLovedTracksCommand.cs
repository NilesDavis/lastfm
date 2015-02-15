using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Api.Helpers;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using IF.Lastfm.Core.Api.Commands;
using IF.Lastfm.Core.Api;
using IF.Lastfm.Core;

namespace IF.Lastfm.Core.Api.Commands.UserApi
{
    internal class UserGetLovedTracksCommand : GetAsyncCommandBase<PageResponse<LastTrack>>
    {
        public string Username { get; private set; }

        public DateTime? From { get; set; }

        public UserGetLovedTracksCommand(ILastAuth auth, string username) : base(auth)
        {
            Method = "user.getLovedTracks";

            Username = username;
        }

        public override void SetParameters()
        {
            Parameters.Add("user", Username);
            AddPagingParameters();
            DisableCaching();
        }

        public async override Task<PageResponse<LastTrack>> HandleResponse(HttpResponseMessage response)
        {
            var json = await response.Content.ReadAsStringAsync();

            LastFmApiError error;
            if (LastFm.IsResponseValid(json, out error) && response.IsSuccessStatusCode)
            {
                var jtoken = JsonConvert.DeserializeObject<JToken>(json).SelectToken("lovedtracks");
                var itemsToken = jtoken.SelectToken("track");
                var attrToken = jtoken.SelectToken("@attr");

                return PageResponse<LastTrack>.CreateSuccessResponse(itemsToken, attrToken, LastTrack.ParseJToken, LastPageResultsType.Attr);
            }
            else
            {
                return LastResponse.CreateErrorResponse<PageResponse<LastTrack>>(error);
            }
        }
    }
}
