using System;
using System.Globalization;
using System.IO;

namespace modmailbot
{
    internal class Settings
    {
        public static void Logger(string Content, string LogType)
        {
            int error = 0;
        spawn:
            try
            {
                if (!File.Exists("log"))
                    File.Create("log");
                string logtext = File.ReadAllText("log") + $"[{DateTime.Now.ToString(new CultureInfo("de-DE"))}] {LogType}: {Content}";
                File.WriteAllText("log", logtext);
            }
            catch (Exception ex)
            {
                error++;
                if (error == 20) Console.WriteLine(ex);
                else goto spawn;
            }
        }

        public static string BotToken = "Bot ODIyMTIzNjY5NzY1MzU3NjQ5.YFNsVQ.PSMCCJQZ55LF5znvK1IF1eyUtH0";
        public static string Prefix = "!";


        public static ulong MainServerID = 822123785200074792;
        public static ulong SupportServerID = 822123821963280414;


        public static ulong TicketCategoryID = 822125911419650058;


        public static ulong BlacklistChannelID = 822126041631555644;
        public static ulong LogChannelID = 822125921711423519;


        public static ulong ResetRoleID = 822125960421834763;
        public static ulong OwnerRoleID = 822125961436463144;


        public static int TicketID;


        public static string WelcomeMessage = "u made ticket";
        public static string CloseMessage = "Thank you for contacting Apple Support.\nYour ticket has been **CLOSED**\n\n__DO NOT__ respond to this unless you wish to reopen a new ticket.\n";
    }
}
