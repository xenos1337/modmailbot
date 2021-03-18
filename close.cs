using Discord;
using Discord.Commands;
using Discord.Gateway;
using System;

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
                        channel.Delete();
                        LogChannel.SendMessage($"{channel.Name} has been closed by <@!{message.Author.User.Id}>");
                    }
                    else { }
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
