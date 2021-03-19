using Discord;
using Discord.Commands;
using Discord.Gateway;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
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
                                    if (GuildChannels[i].Type == ChannelType.Category) i++;
                                    TextChannel cTextChannel = GuildChannels[i].ToTextChannel();
                                    ulong cTextChannelTopic = ulong.Parse(cTextChannel.Topic);
                                    client.CreateDM(cTextChannelTopic).ToDMChannel().SendMessage($"", false, embed);
                                    string fileName = $"Logs\\{channel.Name}.txt";
                                    FileStream fs = File.Create(fileName);
                                    fs.Close();
                                    StreamWriter write69 = new StreamWriter($"Logs\\{channel.Name}.txt");
                                    write69.WriteLine("Apple Support 2.0");
                                    write69.WriteLine("");
                                    write69.WriteLine($"User: {cTextChannelTopic}");
                                    write69.WriteLine($"Date: {DateTime.Now.ToString(new CultureInfo("de-DE"))}");
                                    write69.WriteLine("--------------------------------");
                                    write69.WriteLine("");
                                    try
                                    {
                                        foreach (var guierhguiherug in client.GetChannelMessages(message.Channel.Id))
                                        {
                                            string niggor69 = guierhguiherug.Author.User.Username.ToString() + ": " + guierhguiherug.Content + $"( {DateTime.Now.Hour.ToString(new CultureInfo("de-DE"))}.{DateTime.Now.Minute.ToString(new CultureInfo("de-DE"))}.{DateTime.Now.Second.ToString(new CultureInfo("de-DE"))} )";
                                            write69.WriteLine(niggor69);
                                        }
                                    }
                                    catch (InvalidOperationException)
                                    {
                                        write69.WriteLine("This ticket contained no text");
                                    }
                                    write69.WriteLine("");
                                    write69.WriteLine("--------------------------------");
                                    write69.WriteLine($"Staff Closed: {message.Author.User.Username} ({message.Author.User.Id})");
                                    write69.WriteLine($"Time: {DateTime.Now.ToString(new CultureInfo("de-DE"))}");
                                    write69.Close();
                                    channel.Delete();
                                    LogChannel.SendFile($"Logs\\{channel.Name}.txt", $"{channel.Name} has been closed by <@!{message.Author.User.Id}>");
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
