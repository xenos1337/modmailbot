using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Discord;
using Discord.Gateway;
using IniParser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace modmailbot
{
    class Program
    {
        static public string desc;

        static void Main()
        {
            try
            {
                Discord.DiscordConfig config = new Discord.DiscordConfig();
                DiscordSocketClient client = new DiscordSocketClient();
                client.OnLoggedIn += Client_OnLoggedIn;
                client.OnMessageReceived += Client_OnMessageReceived;
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
            if (args.Message.Content == "") returnstr = args.Message.Attachment.ProxyUrl;
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



        public static void Client_OnMessageReceived(DiscordSocketClient client, MessageEventArgs args)
        {
            try
            {
                DiscordChannel channel1 = client.GetChannel(args.Message.Channel.Id);
                string channeltype = channel1.Type.ToString();

                if (channeltype == "DM" && args.Message.Author.User.Type != DiscordUserType.Bot)
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
                        EmbedMaker EMaker = new EmbedMaker()
                        {
                            Title = "Ticket " + Settings.TicketID,
                            Description = "👤 User\n" +
                            $"<@{args.Message.Author.User.Id}>\n" +
                            $"_({args.Message.Author.User.Id})_\n\n" +
                            $"📄 Reason\n" +
                            $"`{_MessageContent}`",
                            Color = System.Drawing.Color.Red
                        };
                        //client.CreateDM(args.Message.Author.User.Id).ToDMChannel().SendMessage($"Created Ticket-{Settings.TicketID}, please patiently for a response!");
                        client.SendMessage(CurrentChannel.Id, "", false, EMaker);
                        Settings.TicketID++;
                    }
                    else
                    {
                        client.SendMessage(CurrentChannel.Id, $"**{args.Message.Author.User.Username}#{args.Message.Author.User.Discriminator}**: {args.Message.Content}", false);
                    }


                    data["Ticket"]["TicketID"] = Settings.TicketID.ToString();
                    parser.WriteFile("config.xenos", data);

                }
                else if (args.Message.Author.User.Type != DiscordUserType.Bot && channel1.Name.StartsWith("ticket-"))
                {
                    client.CreateDM(args.Message.Author.User.Id).ToDMChannel().SendMessage($"{args.Message.Content}");
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }
        }

        /*SocketGuild guild = client.GetCachedGuild(Settings.SupportServerID);

        string[] arrayyyy = File.ReadAllLines("opentickets.txt");
        string ticketnummyyyy = string.Join("", arrayyyy);

        if (ticketnummyyyy.Contains(args.Message.Author.User.Id.ToString()))
        {
            SocketGuild servor = client.GetCachedGuild(Settings.SupportServerID);
            foreach (var nigga in servor.GetChannels())
            {
                if (nigga.Type.ToString() == "Text")
                {
                    if (nigga.Name.StartsWith("ticket"))
                    {
                        TextChannel sefsefesfs = client.GetChannel(nigga.Id).ToTextChannel();
                        foreach (var guierhguiherug in client.GetChannelMessages(sefsefesfs.Id))
                        {
                            if (guierhguiherug.Content.Contains(args.Message.Author.User.Id.ToString()))
                            {
                                if (guierhguiherug.Author.User.Id == client.User.Id)
                                {
                                    StreamWriter write69 = new StreamWriter("opentickets.txt");
                                    write69.Flush();
                                    string niggor69 = args.Message.Author.User.Id.ToString();
                                    write69.WriteLine(niggor69);
                                    write69.Close();
                                    TextChannel sefsefeswrgwfs = client.GetChannel(guierhguiherug.Channel.Id).ToTextChannel();
                                    sefsefeswrgwfs.SendMessage(args.Message.Content);
                                }
                                else { }
                            }
                            else { }
                        }
                    }
                    else { }
                }
                else { }
            }
        }



        string[] array = File.ReadAllLines("config.txt");
        string ticketnummy = string.Join("", array);
        Settings.TicketID = Convert.ToInt32(ticketnummy);
        Settings.TicketID += 1;
        Thread.Sleep(500);
        StreamWriter write = new StreamWriter("config.txt");
        write.Flush();
        string niggor = Settings.TicketID.ToString();
        write.WriteLine(niggor);
        write.Close();



        if (args.Message.Author.User.Type.ToString() != "User")
            return;




        var TicketChannel = guild.CreateChannel($"ticket-{Settings.TicketID}", ChannelType.Text, Settings.TicketCategoryID);

        TicketChannel.Modify(new TextChannelProperties() { Topic = args.Message.Author.User.Id.ToString() });

        if (args.Message.Content == "")
            desc = args.Message.Attachment.Url;
        else
            desc = args.Message.Content;



        EmbedMaker embed = new EmbedMaker();
        embed.Color = Color.FromArgb(27, 81, 173);
        embed.Title = $"Ticket {Settings.TicketID}";
        embed.Description =
            $"👤 User\n" +
            $"<@{args.Message.Author.User.Id}>\n" +
            $"_({args.Message.Author.User.Id})_\n\n" +
            $"📄 Message\n" +
            $"`{desc}`";
        embed.ThumbnailUrl = "https://i.imgur.com/RT1TEDh.png";
        embed.Footer.Text = "Apple Support 2.1";
        embed.Footer.IconUrl = "https://cdn.discordapp.com/avatars/780516738948268053/e196254270adbfac834d794c11f847ef.webp";
        TicketChannel.ToTextChannel().SendMessage("", false, embed);
        TicketChannel.ToTextChannel().SendMessage($"{args.Message.Author.User.Id}");*/

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
