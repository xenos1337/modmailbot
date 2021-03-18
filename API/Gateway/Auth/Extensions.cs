﻿namespace Discord.Gateway
{
    public static class GatewayAuthExtensions
    {
        /// <summary>
        /// Logs intot he gateway
        /// </summary>
        internal static void LoginToGateway(this DiscordSocketClient client)
        {
            var identification = new GatewayIdentification()
            {
                Token = client.Token,
                Properties = client.Config.SuperProperties,
                Intents = client.Config.Intents
            };

            client.Send(GatewayOpcode.Identify, identification);
        }


        internal static void Resume(this DiscordSocketClient client)
        {
            client.Send(GatewayOpcode.Resume, new GatewayResume(client));
        }
    }
}
