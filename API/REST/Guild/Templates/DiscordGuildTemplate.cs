﻿using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord
{
    public class DiscordGuildTemplate : ControllableEx
    {
        public DiscordGuildTemplate()
        {
            OnClientUpdated += (sender, e) =>
            {
                SourceGuild.SetClient(Client);
                Template.SetClient(Client);
            };
            JsonUpdated += (sender, json) =>
            {
                Template.SetJson(json.Value<JObject>("serialized_source_guild"));
            };
        }

        [JsonProperty("code")]
        public string Code { get; private set; }


        [JsonProperty("name")]
        public string Name { get; private set; }


        [JsonProperty("usage_count")]
        public int Usages { get; private set; }


        [JsonProperty("creator")]
        public DiscordUser Creator { get; private set; }


        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; private set; }


        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; private set; }


        [JsonProperty("source_guild_id")]
        private ulong _guildId;

        public MinimalGuild SourceGuild
        {
            get
            {
                return new MinimalGuild(_guildId).SetClient(Client);
            }
        }


        private DiscordTemplateGuild _guild;
        [JsonProperty("serialized_source_guild")]
        public DiscordTemplateGuild Template
        {
            get
            {
                return _guild;
            }
            private set
            {
                _guild = value;

                _guild.SetGuildId(_guildId);
            }
        }


        public void Update()
        {
            var template = Client.GetGuildTemplate(Code);
            Name = template.Name;
            Usages = template.Usages;
            Creator = template.Creator;
            _guildId = template._guildId;
            Template = template.Template;
        }
    }
}
