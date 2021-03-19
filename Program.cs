using Discord;
using Discord.Gateway;
using Discord.Webhook;
using IniParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace modmailbot
{
    class Program
    {
        static public DiscordMessage DMMessage;

        static void Main()
        {
            try
            {
                Discord.DiscordConfig config = new Discord.DiscordConfig();
                DiscordSocketClient client = new DiscordSocketClient();
                client.OnLoggedIn += Client_OnLoggedIn;
                client.OnMessageReceived += Client_OnMessageReceived;
                client.OnMessageEdited += Client_OnMessageEdited;
                client.CreateCommandHandler(Settings.Prefix);
                client.Login(Settings.BotToken);
                Thread.Sleep(-1);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ReadLine();
            }

        }


        private static string GetMessage(DiscordSocketClient client, MessageEventArgs args)
        {
            string returnstr;
            if (args.Message.Content == "") returnstr = args.Message.Attachment.Url;
            else returnstr = args.Message.Content;
            return returnstr;
        }


        public static GuildChannel CheckForExistingTicket(DiscordSocketClient client, MessageEventArgs args)
        {
            GuildChannel CurrentChannel = new GuildChannel();
            List<Discord.GuildChannel> GuildChannels = client.GetGuildChannels(Settings.SupportServerID).ToList();
            for (int i = 0; GuildChannels.Count > i; i++)
            {
                try
                {
                    if (GuildChannels[i].Type == ChannelType.Category) i++;
                    TextChannel cTextChannel = GuildChannels[i].ToTextChannel();
                    string cTextChannelTopic = cTextChannel.Topic.ToString();
                    if (cTextChannelTopic.Contains(args.Message.Author.User.Id.ToString()))
                    {
                        CurrentChannel = GuildChannels[i];
                        continue;
                    }
                }
                catch (Exception)
                {
                    i++;
                }
            }
            return CurrentChannel;
        }

        public static void Client_OnMessageEdited(DiscordSocketClient client, MessageEventArgs args)
        {
            try
            {
                DiscordChannel channel1 = client.GetChannel(args.Message.Channel.Id);

                GuildChannel CurrentChannel = new GuildChannel();
                List<Discord.GuildChannel> GuildChannels = client.GetGuildChannels(Settings.SupportServerID).ToList();
                for (int i = 0; GuildChannels.Count > i; i++)
                {
                    try
                    {

                        if (GuildChannels[i].Type == ChannelType.Category) i++;
                        TextChannel cTextChannel = GuildChannels[i].ToTextChannel();
                        ulong cTextChannelTopic = ulong.Parse(cTextChannel.Topic);
                        PrivateChannel chanol = client.CreateDM(cTextChannelTopic).ToDMChannel();
                        client.EditMessage(chanol, DMMessage.Id, args.Message.Content);
                        Console.WriteLine(chanol);
                        Console.WriteLine(DMMessage.Id);
                        Console.WriteLine(args.Message.Content);
                        Console.WriteLine(cTextChannelTopic);
                        //CurrentChannel = GuildChannels[i];
                        //continue;
                    }
                    catch (Exception) { i++; }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }
        }

        public static void Client_OnMessageReceived(DiscordSocketClient client, MessageEventArgs args)
        {
            try
            {
                DiscordChannel channel1 = client.GetChannel(args.Message.Channel.Id);
                string channeltype = channel1.Type.ToString();

                if (channeltype == "DM" && args.Message.Author.User.Type != DiscordUserType.Bot && args.Message.Author.User.Type != DiscordUserType.Webhook)
                {

                    var parser = new FileIniDataParser();
                    IniParser.Model.IniData data = null;

                    if (!File.Exists("config.xenos")) File.Create("config.xenos");
                    if (File.ReadAllText("config.xenos").Length < 3)
                    {
                        data = parser.ReadFile("config.xenos");
                        data["Ticket"]["TicketID"] = Settings.TicketID.ToString();
                        parser.WriteFile("config.xenos", data);
                    }

                    try
                    {
                        data = parser.ReadFile("config.xenos");
                    }
                    catch (Exception)
                    {
                        data = parser.ReadFile("config.xenos");
                        data["Ticket"]["TickerID"] = Settings.TicketID.ToString();
                        parser.WriteFile("config.xenos", data);
                    }

                    Settings.TicketID = Int32.Parse(data["Ticket"]["TicketID"]);

                    string _MessageContent = GetMessage(client, args);

                    DiscordGuild ourguild = client.GetGuild(Settings.SupportServerID);
                    GuildChannel CurrentChannel = CheckForExistingTicket(client, args);

                    bool HasTicketOpenAlready = false;
                    if (CurrentChannel == 0) HasTicketOpenAlready = false;
                    else HasTicketOpenAlready = true;

                    if (!HasTicketOpenAlready)
                    {
                        CurrentChannel = ourguild.CreateChannel($"ticket-{Settings.TicketID}", ChannelType.Text, Settings.TicketCategoryID);
                        TextChannel gChannel = CurrentChannel.ToTextChannel();
                        gChannel.Modify(new TextChannelProperties() { Topic = args.Message.Author.User.Id.ToString() });

                        EmbedMaker embed = new EmbedMaker();
                        embed.Color = Color.FromArgb(27, 81, 173);
                        embed.Title = $"Ticket {Settings.TicketID}";
                        embed.Description =
                            $"👤 User\n" +
                            $"<@{args.Message.Author.User.Id}>\n" +
                            $"_({args.Message.Author.User.Id})_\n\n" +
                            $"📄 Reason\n" +
                            $"`{_MessageContent}`";
                        embed.ThumbnailUrl = "https://i.imgur.com/RT1TEDh.png";
                        embed.Footer.Text = $"Apple Support 2.0 • {args.Message.SentAt}";
                        embed.Footer.IconUrl = "https://cdn.discordapp.com/avatars/780516738948268053/e196254270adbfac834d794c11f847ef.webp";
                        client.SendMessage(CurrentChannel.Id, "", false, embed);
                        DiscordImage ProfilePicture = args.Message.Author.User.Avatar.Download();
                        var niggito = gChannel.CreateWebhook(new DiscordWebhookProperties() { Name = args.Message.Author.User.Username, Avatar = ProfilePicture });
                        foreach (var GetWebhooks in gChannel.GetWebhooks())
                        {
                            GetWebhooks.SendMessage(_MessageContent);
                        }
                        Settings.TicketID++;
                    }
                    else
                    {

                        TextChannel gChannel = CurrentChannel.ToTextChannel();
                        foreach (var GetWebhooks in gChannel.GetWebhooks())
                        {
                            GetWebhooks.SendMessage(_MessageContent);
                        }
                        //client.SendMessage(CurrentChannel.Id, $"**{args.Message.Author.User.Username}#{args.Message.Author.User.Discriminator}**: {_MessageContent}", false);
                    }


                    data["Ticket"]["TicketID"] = Settings.TicketID.ToString();
                    parser.WriteFile("config.xenos", data);

                }
                else if (args.Message.Author.User.Type != DiscordUserType.Bot && args.Message.Author.User.Type != DiscordUserType.Webhook && channel1.Name.StartsWith("ticket-"))
                {
                    string _MessageContent = GetMessage(client, args);

                    if (!args.Message.Content.StartsWith(".") && !args.Message.Content.Contains("!close"))
                    {
                        GuildChannel CurrentChannel = new GuildChannel();
                        List<Discord.GuildChannel> GuildChannels = client.GetGuildChannels(Settings.SupportServerID).ToList();
                        for (int i = 0; GuildChannels.Count > i; i++)
                        {
                            try
                            {

                                if (GuildChannels[i].Name == channel1.Name)
                                {

                                    if (GuildChannels[i].Type == ChannelType.Category) i++;
                                    TextChannel cTextChannel = GuildChannels[i].ToTextChannel();
                                    ulong cTextChannelTopic = ulong.Parse(cTextChannel.Topic);
                                    client.CreateDM(cTextChannelTopic).ToDMChannel().SendMessage($"{_MessageContent}");
                                }
                                //CurrentChannel = GuildChannels[i];
                                //continue;
                            }
                            catch (Exception) { i++; }
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }
        }

        private static void Client_OnLoggedIn(DiscordSocketClient client, LoginEventArgs args)
        {
            try
            {
                //client.DeleteMessage(259853201359110144, 822270036089110529);
                //client.EditMessage(259853201359110144, 822270036089110529, "same");
                Console.WriteLine($"Logged in as: {client.User.Username}#{client.User.Discriminator} | {client.User.Id}");
                Console.WriteLine($"Guilds: {client.GetGuilds().Count}");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
    }
}
