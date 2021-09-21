﻿using System;
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
                Console.WriteLine(ex.Message);
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
                Console.WriteLine(ex.Message);
                goto spawn;
            }
        }

        public static string BotToken = "Bot ODk1NDEyOTQx..."; 
        public static string Prefix = "!";


        // all of this isn't implemented yet i think
        public static ulong MainServerID = 822123785200074792;
        public static ulong SupportServerID = 822123821963280414;


        public static ulong TicketCategoryID = 822563577873170482;


        public static ulong BlacklistChannelID = 822579668632469524;
        public static ulong LogChannelID = 822579067148304394;


        public static ulong ResetRoleID = 822125960421834763;
        public static ulong OwnerRoleID = 822125961436463144;


        public static int TicketID;


        public static string WelcomeMessage = "u made ticket";
        public static string CloseMessage = "Thank you for contacting Apple Support.\nYour ticket has been **CLOSED**\n\n__DO NOT__ respond to this unless you wish to reopen a new ticket.\n";
    }
}
