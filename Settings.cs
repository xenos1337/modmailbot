using System;
using System.Globalization;
using System.IO;
using System.Threading;

namespace modmailbot
{
    internal class Settings
    {
        public static void CreateLog(string FileName, string User)
        {
        spawn:
            try
            {

                if (!File.Exists(FileName))
                    File.Create(FileName);
                string logtext = File.ReadAllText(FileName) + $"Apple Support 2.0\n\nUser: {User}--------------------------------";
                File.WriteAllText(FileName, logtext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                goto spawn;
            }
        }

        public static void AddToLog(string FileName, string User, string Content)
        {
        spawn:
            try
            {
                if (!File.Exists(FileName))
                    CreateLog(FileName, User);
                string logtext = File.ReadAllText(FileName) + $"\n {User}: {Content}";
                File.WriteAllText(FileName, logtext);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                goto spawn;
            }
        }

        public static string BotToken = "Bot ODIyMTIzNjY5NzY1MzU3NjQ5.YFNsVQ.WB2Z9qqGd5q-g-eWKjaidYTJMx0";
        public static string Prefix = "!";


        public static ulong MainServerID = 788150383952134175;
        public static ulong SupportServerID = 828703350740353025;


        public static ulong TicketCategoryID = 828704539103461437;
        public static ulong LogChannelID = 828704425102016512;


        public static int TicketID;

        public static string CloseMessage = "Thank you for contacting Apple Support.\nYour ticket has been **CLOSED**\n\n__DO NOT__ respond to this unless you wish to reopen a new ticket.\n";
    }
}
