using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using Discord;
using Newtonsoft.Json;

namespace YahurrBot
{
    class Program
    {
        static void Main ( string[] args )
        {
            new Program ().Start ();
        }

        DiscordClient client;

        public void Start ()
        {
            client = new DiscordClient ();

            client.MessageReceived += ( s, e ) =>
            {
                if (!e.Message.IsAuthor)
                {
                    string message = e.Message.Text + " ";
                    string[] commdands = message.ToLower ().Split (' ');

                    Module.Command (commdands, e);
                }
            };

            client.UserUpdated += ( s, p ) =>
            {

                Module.UpdateProfile (p);
            };

            client.UserJoined += ( s, p ) =>
            {
                Module.JoinedUser (p);
            };


            client.ExecuteAndWait (async () =>
            {
                Console.WriteLine ("Connect to discord...");

                await client.Connect ("MjI4NDYzNTEyMzQ1NzcyMDMy.Csxslw.0Khe_VrvEdR86XWtx4I5lUnArKU", TokenType.Bot);

                Console.WriteLine ("Bot is connected with key: MjI4NDYzNTEyMzQ1NzcyMDMy.CsxZjg.ilg6p_QgxMCC7t-9IMcc5uyl_6Q.");
                Console.WriteLine ("");

                Console.WriteLine ("Loading modules...");

                // Waits for client to fully load. (No longer an issue but keeping the code.)
                await Task.Run (() =>
                {
                    while (true)
                    {
                        if (client.Servers.Count () > 0 && client.Servers.First ().UserCount > 3)
                        {
                            break;
                        }
                    }
                });

                await Task.Run (() =>
                {
                    int modules = Module.LoadModules (client);
                    AppDomain.CurrentDomain.ProcessExit += new EventHandler (Module.ExitProgram);

                    client.SetGame ("with jews.");

                    Console.WriteLine (modules + " modules loaded.");
                    Console.WriteLine ("");
                });

                while (true)
                {
                    string message = Console.ReadLine ();
                    string[] commands = message.Split (' ');

                    Module.ConsoleCommand (commands);

                    switch (commands[0])
                    {
                        case "setgame":
                            client.SetGame (new Discord.Game (commands[1].Replace ('-', ' ')));
                            break;
                        default:
                            break;
                    }
                }
            });
        }
    }
}