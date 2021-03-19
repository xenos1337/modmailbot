﻿using Leaf.xNet;

namespace Discord
{
    public class DiscordConfig
    {
        public ProxyClient Proxy { get; set; }
        public string UserAgent
        {
            get { return SuperProperties.UserAgent; }
            set { SuperProperties.UserAgent = value; }
        }
        public SuperProperties SuperProperties { get; set; } = SuperProperties.FromBase64("eyJvcyI6IldpbmRvd3MiLCJicm93c2VyIjoiRmlyZWZveCIsImRldmljZSI6IiIsInN5c3RlbV9sb2NhbGUiOiJkZSIsImJyb3dzZXJfdXNlcl9hZ2VudCI6Ik1vemlsbGEvNS4wIChXaW5kb3dzIE5UIDEwLjA7IFdpbjY0OyB4NjQ7IHJ2Ojg2LjApIEdlY2tvLzIwMTAwMTAxIEZpcmVmb3gvODYuMCIsImJyb3dzZXJfdmVyc2lvbiI6Ijg2LjAiLCJvc192ZXJzaW9uIjoiMTAiLCJyZWZlcnJlciI6IiIsInJlZmVycmluZ19kb21haW4iOiIiLCJyZWZlcnJlcl9jdXJyZW50IjoiIiwicmVmZXJyaW5nX2RvbWFpbl9jdXJyZW50IjoiIiwicmVsZWFzZV9jaGFubmVsIjoic3RhYmxlIiwiY2xpZW50X2J1aWxkX251bWJlciI6ODAwNDEsImNsaWVudF9ldmVudF9zb3VyY2UiOm51bGx9");
        public bool GetFingerprint { get; set; } = true;
        public string ApiBaseUrl { get; set; } = "https://discord.com/api/v6";
        public bool RetryOnRateLimit { get; set; } = true;
    }
}
