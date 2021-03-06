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
using System.Net;
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
                DiscordSocketClient client = new DiscordSocketClient(new DiscordSocketConfig { Cache = true, RetryOnRateLimit = true, Intents = DiscordGatewayIntent.DirectMessages | DiscordGatewayIntent.GuildWebhooks | DiscordGatewayIntent.Guilds | DiscordGatewayIntent.GuildMessages, ApiBaseUrl = "https://discord.com/api/v9" } );
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

                    if (!File.Exists("config.ini")) File.Create("config.ini");
                    if (File.ReadAllText("config.ini").Length < 3)
                    {
                        data = parser.ReadFile("config.ini");
                        data["Ticket"]["TicketID"] = Settings.TicketID.ToString();
                        parser.WriteFile("config.ini", data);
                    }

                    try
                    {
                        data = parser.ReadFile("config.ini");
                    }
                    catch (Exception)
                    {
                        data = parser.ReadFile("config.ini");
                        data["Ticket"]["TickerID"] = Settings.TicketID.ToString();
                        parser.WriteFile("config.ini", data);
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
                        embed.Footer.Text = $"Apple Support 2.0 • {args.Message.SentAt}"; // idk why i made it to bad it's very old
                        embed.Footer.IconUrl = "https://cdn.discordapp.com/avatars/780516738948268053/e196254270adbfac834d794c11f847ef.webp";
                        client.SendMessage(CurrentChannel.Id, "", false, embed);

                        try
                        {
                            Console.WriteLine("failed 1");
                            WebClient WBClient = new WebClient();
                            Console.WriteLine("failed 2");
                            byte[] imageData = WBClient.DownloadData(args.Message.Author.User.Avatar.Url);
                            Console.WriteLine("failed 3");
                            MemoryStream MemStream = new MemoryStream(imageData);
                            Console.WriteLine("failed 4");
                            DiscordImage DCImage = Image.FromStream(MemStream);
                            Console.WriteLine("failed 5");
                            gChannel.CreateWebhook(new DiscordWebhookProperties() { Name = args.Message.Author.User.Username });
                            Console.WriteLine("failed 5.1");
                            var CWebhook = gChannel.CreateWebhook(new DiscordWebhookProperties() { Name = args.Message.Author.User.Username });
                            Console.WriteLine("failed 6");
                            CWebhook.Modify(new DiscordWebhookProperties() { Avatar = DCImage });
                            Console.WriteLine("failed 7");
                            foreach (var GetWebhooks in gChannel.GetWebhooks())
                            {
                                Console.WriteLine("failed 8");
                                GetWebhooks.SendMessage(_MessageContent);
                                Console.WriteLine("failed 9");
                            }
                            Console.WriteLine("failed 10");
                            Settings.TicketID++;
                            Console.WriteLine("failed 11");
                        }
                        catch (System.Net.WebException)
                        {
                            DiscordImage DCImage = Image.FromFile(@"E:\Downloads\discord.png"); // if you wanna update this have fun https://cdn.discordapp.com/attachments/834342154993926187/889918069625999470/discord.png
                            var CWebhook = gChannel.CreateWebhook(new DiscordWebhookProperties() { Name = args.Message.Author.User.Username });
                            CWebhook.Modify(new DiscordWebhookProperties() { Avatar = DCImage });
                            foreach (var GetWebhooks in gChannel.GetWebhooks())
                            {
                                GetWebhooks.SendMessage(_MessageContent);
                            }
                            Settings.TicketID++;
                            Console.WriteLine("failed 199");
                        }
                    }
                    else
                    {
                        TextChannel gChannel = CurrentChannel.ToTextChannel();
                        foreach (var GetWebhooks in gChannel.GetWebhooks())
                        {
                            GetWebhooks.SendMessage(_MessageContent);
                        }
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
