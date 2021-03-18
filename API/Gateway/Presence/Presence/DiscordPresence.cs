﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord.Gateway
{
    public class DiscordPresence : ControllableEx
    {
        public DiscordPresence()
        {
            OnClientUpdated += (sender, e) =>
            {
                Activities.SetClientsInList(Client);
            };
            JsonUpdated += (sender, json) =>
            {
                Activities = json.Value<JArray>("activities").PopulateListJson<UserActivity>();
            };
        }


        [JsonProperty("guild_id")]
        protected ulong? _guildId;


        public bool IsGuild
        {
            get
            {
                return _guildId.HasValue;
            }
        }


        [JsonProperty("user")]
        private readonly JObject _user;

        public ulong UserId
        {
            get { return _user["id"].ToObject<ulong>(); }
        }


        [JsonProperty("activities")]
        public IReadOnlyList<UserActivity> Activities { get; private set; }


        [JsonProperty("status")]
        private string _status;

        public UserStatus Status
        {
            get
            {
                return UserStatusConverter.FromString(_status);
            }
            set
            {
                _status = UserStatusConverter.ToString(value);
            }
        }


        [JsonProperty("client_status")]
        public ActiveSessionPlatforms ActivePlatforms { get; private set; }

        
        public DiscordGuildPresence ToGuildPresence()
        {
            if (IsGuild)
                return Json.ToObject<DiscordGuildPresence>().SetJson(Json).SetClient(Client);
            else
                throw new InvalidConvertionException(Client, "This presence is not of a guild member");
        }


        public override string ToString()
        {
            return UserId.ToString();
        }
    }
}
