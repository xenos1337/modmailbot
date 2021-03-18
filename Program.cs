using System;
using System.Drawing;
using System.IO;
using System.Threading;
using Discord;
using Discord.Gateway;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace modmailbot
{
	class Program
	{

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

		public static void Client_OnMessageReceived(DiscordSocketClient client, MessageEventArgs args)
		{
			try
			{
				DiscordChannel channel = client.GetChannel(args.Message.Channel.Id);
				string channeltype = ((PrivateChannel)channel).Type.ToString();

				if (channeltype == "DM")
				{
					string[] array = File.ReadAllLines("config.txt");
					string ticketnummy = string.Join("", array);
					Settings.TicketID = Convert.ToInt32(ticketnummy);
					Console.WriteLine(Settings.TicketID);
					Settings.TicketID += 1;
					StreamWriter write = new StreamWriter("config.txt");
					write.Flush();
					string niggor = Settings.TicketID.ToString();
					write.WriteLine(niggor);
					write.Close();

					GuildChannel GLChannel = new GuildChannel();
					DiscordGuild ourGuild = client.GetGuild(Settings.SupportServerID);

					var ticketCH = ourGuild.CreateChannel($"ticket-{Settings.TicketID}", ChannelType.Text, Settings.TicketCategoryID);


					TextChannel tchannel = client.GetChannel(ticketCH.Id).ToTextChannel();

					EmbedMaker embed = new EmbedMaker();
					embed.Color = Color.FromArgb(27, 81, 173);
					embed.Title = $"Ticket {Settings.TicketID}";
					embed.Description =
						$"👤 User\n" +
						$"<@{args.Message.Author.User.Id}>\n" +
						$"_({args.Message.Author.User.Id})_\n\n" +
						$"📄 Reason\n" +
						$"`{args.Message.Content}`";
					embed.ThumbnailUrl = "https://i.imgur.com/RT1TEDh.png";
					embed.Footer.Text = "Apple Support 2.0";
					embed.Footer.IconUrl = "https://cdn.discordapp.com/avatars/780516738948268053/e196254270adbfac834d794c11f847ef.webp";
					tchannel.SendMessage("", false, embed);

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
				Console.WriteLine($"Logged in as: {client.User}");
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
