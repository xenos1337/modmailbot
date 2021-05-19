using System;
using System.Drawing;
using System.Threading;
using Discord;
using Discord.Gateway;

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
		int currentticketid = 1;

		try
		{
			DiscordChannel channel = client.GetChannel(args.Message.Channel.Id);
			string channeltype = ((PrivateChannel)channel).Type.ToString();

			if (channeltype == "DM")
                	{
				GuildChannel GLChannel = new GuildChannel();
				DiscordGuild ourGuild = client.GetGuild(Settings.SupportServerID);
				var ticketCH = ourGuild.CreateChannel($"ticket-{currentticketid}", ChannelType.Text, Settings.TicketCategoryID);

				TextChannel tchannel = client.GetChannel(ticketCH.Id).ToTextChannel();

				EmbedMaker embed = new EmbedMaker();
				embed.Color = Color.FromArgb(52, 255, 33);
				embed.Title = $"Ticket {currentticketid}";
				embed.Description = 
					$"👤 User\n" +
					$"<@{args.Message.Author.User.Id}>\n" +
					$"_({args.Message.Author.User.Id})_\n\n" +
					$"📄 Reason\n" +
					$"`{args.Message.Content}`";
				tchannel.SendMessage("", false, embed);

				parser.DeleteSetting("modmailbot", "TicketID");
			}
		}
		catch (Exception e)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(e);
			Console.ReadLine();
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
