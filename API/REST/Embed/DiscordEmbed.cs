﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace Discord
{
    public class DiscordEmbed
    {
        public DiscordEmbed()
        {
            Fields = new List<EmbedField>();
            Thumbnail = new EmbedImage();
            Image = new EmbedImage();
            Footer = new EmbedFooter();
            Author = new EmbedAuthor();
        }


        [JsonProperty("title")]
        public string Title { get; internal set; }


        [JsonProperty("url")]
        public string TitleUrl { get; set; }


        [JsonProperty("description")]
        public string Description { get; internal set; }


        [JsonProperty("color")]
        private uint _color;
        public Color Color
        {
            get { return Color.FromArgb((int)_color); }
            set { _color = (uint)Color.FromArgb(0, value.R, value.G, value.B).ToArgb(); }
        }


        [JsonProperty("fields")]
        public IReadOnlyList<EmbedField> Fields { get; internal set; }


        [JsonProperty("video")]
        public EmbedVideo Video { get; private set; }


        [JsonProperty("thumbnail")]
        public EmbedImage Thumbnail { get; private set; }


        [JsonProperty("image")]
        public EmbedImage Image { get; private set; }


        [JsonProperty("footer")]
        public EmbedFooter Footer { get; private set; }


        [JsonProperty("author")]
        public EmbedAuthor Author { get; private set; }


        public override string ToString()
        {
            return Title;
        }
    }
}
