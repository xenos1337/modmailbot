using Discord;
using Discord.Commands;
using Discord.Gateway;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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
                        EmbedMaker embed = new EmbedMaker();
                        embed.Color = Color.FromArgb(27, 81, 173);
                        embed.Title = $"Ticket Closed";
                        embed.Description =
                            $"Thank you for contacting Apple Support.\n" +
                            $"Your ticket has been **CLOSED**\n\n" +
                            $"__DO NOT__ respond to this unless you wish to reopen a new ticket.\n";
                        embed.ThumbnailUrl = "https://i.imgur.com/RT1TEDh.png";
                        embed.Footer.Text = "Apple Support 2.0";
                        embed.Footer.IconUrl = "https://cdn.discordapp.com/avatars/780516738948268053/e196254270adbfac834d794c11f847ef.webp";

                        client.CreateDM(message.Author.User.Id).ToDMChannel().SendMessage($"", false, embed);
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
