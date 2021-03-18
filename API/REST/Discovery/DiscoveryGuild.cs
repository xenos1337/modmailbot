﻿using Discord.Gateway;
using Newtonsoft.Json;
using System;

namespace Discord
{
    public class DiscoveryGuild : BaseGuild
    {
        [JsonProperty("description")]
        public string Description { get; protected set; }

        [JsonProperty("approximate_presence_count")]
        public int OnlineMembers { get; private set; }


        [JsonProperty("approximate_member_count")]
        public int Members { get; private set; }


        [JsonProperty("premium_subscription_count")]
        public int PremiumSubscriptions { get; private set; }


        [JsonProperty("preferred_locale")]
        private readonly string _locale;

        public DiscordLanguage PreferredLanguage
        {
            get
            {
                return LanguageConverter.FromString(_locale);
            }
        }


        public DiscordGuild Lurk()
        {
            if (Client.GetType() == typeof(DiscordSocketClient))
                return ((DiscordSocketClient)Client).LurkGuild(Id);
            else
                throw new NotSupportedException("This method is only available for socket clients.");
        }
    }
}
