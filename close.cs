using Discord;
using Discord.Commands;
using Discord.Gateway;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace modmailbot
{
    class close
    {
        [Command("close")]
        public class CloseCommand : ICommand
        {
            public void Execute(DiscordSocketClient client, DiscordMessage message)
            {
                try
                {
                    message.Delete();
                    TextChannel channel = client.GetChannel(message.Channel.Id).ToTextChannel();
                    TextChannel LogChannel = client.GetChannel(Settings.LogChannelID).ToTextChannel();
                    if (channel.Name.StartsWith("ticket-"))
                    {
                        EmbedMaker embed = new EmbedMaker();
                        embed.Color = Color.FromArgb(27, 81, 173);
                        embed.Title = $"Ticket Closed";
                        embed.Description = $"{Settings.CloseMessage}";
                        embed.ThumbnailUrl = "https://i.imgur.com/RT1TEDh.png";
                        embed.Footer.Text = "Apple Support 2.0";
                        embed.Footer.IconUrl = "https://cdn.discordapp.com/avatars/780516738948268053/e196254270adbfac834d794c11f847ef.webp";

                        GuildChannel CurrentChannel = new GuildChannel();
                        List<Discord.GuildChannel> GuildChannels = client.GetGuildChannels(Settings.SupportServerID).ToList();
                        for (int i = 0; GuildChannels.Count > i; i++)
                        {
                            try
                            {
                                if (GuildChannels[i].Name == channel.Name)
                                {
                                    channel.Delete();
                                    LogChannel.SendMessage($"{channel.Name} has been closed by <@!{message.Author.User.Id}>");
                                    if (GuildChannels[i].Type == ChannelType.Category) i++;
                                    TextChannel cTextChannel = GuildChannels[i].ToTextChannel();
                                    ulong cTextChannelTopic = ulong.Parse(cTextChannel.Topic);
                                    client.CreateDM(cTextChannelTopic).ToDMChannel().SendMessage($"", false, embed);
                                }
                            }
                            catch (Exception) { i++; }
                        }

                    }
                }
                catch (DiscordHttpException e)
                {
                    Console.WriteLine(e.ErrorMessage);
                }
                catch (InvalidConvertionException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
