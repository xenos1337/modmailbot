﻿using Newtonsoft.Json;
using System;

namespace Discord
{
    public class PartialEmoji : Controllable
    {
        [JsonProperty("id")]
        public ulong? Id { get; private set; }


        [JsonProperty("name")]
        public string Name { get; protected set; }


        [JsonProperty("animated")]
        public bool Animated { get; private set; }


        public DiscordEmojiCDNImage Icon
        {
            get
            {
                if (Id.HasValue)
                    return new DiscordEmojiCDNImage(Id.Value);
                else
                    return null;
            }
        }


        public bool Custom
        {
            get { return Id != null; }
        }


        public string AsMessagable()
        {
            if (Custom)
                return "<" + (Animated ? "a" : "") + $":{Name}:{Id}>";
            else
                throw new NotSupportedException("Emoji must be custom to be converted. To convert standard emojis, get their unicode character");
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
