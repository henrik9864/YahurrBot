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

        BoyPoints boyBot;
        GameCounter gameCounter;

        public void Start ()
        {
            client = new DiscordClient ();

            client.MessageReceived += ( s, e ) =>
            {
                if (!e.Message.IsAuthor)
                {
                    string message = e.Message.Text + " ";
                    string[] commdands = message.ToLower ().Split (' ');

                    boyBot.ParseCommands (commdands, e);
                }
            };

            client.ProfileUpdated += ( s, p ) =>
            {
                gameCounter.ProfileUpdate (p);
            };

            client.ExecuteAndWait (async () =>
            {
                Console.WriteLine ("Connect to discord...");

                await client.Connect ("MjI4NDYzNTEyMzQ1NzcyMDMy.Csxslw.0Khe_VrvEdR86XWtx4I5lUnArKU", TokenType.Bot);

                // Waits for client to fully load.
                await Task.Run (() =>
                {
                    while (true)
                    {
                        if (client.Servers.Count () > 0 && client.Servers.First().UserCount > 3)
                        {
                            break;
                        }
                    }
                });

                Console.WriteLine ("Bot is connected with key: MjI4NDYzNTEyMzQ1NzcyMDMy.CsxZjg.ilg6p_QgxMCC7t-9IMcc5uyl_6Q.");
                Console.WriteLine ("");

                await Task.Run (() =>
                {

                    Console.WriteLine ("Loading modules...");

                    boyBot = new BoyPoints (client);
                    gameCounter = new GameCounter (client);

                    Console.WriteLine ("Modules loaded.");
                });

                await Task.Run (() =>
                {
                    Console.WriteLine ("Loading goodboy points...");

                    boyBot.LoadPoints ();

                    Console.WriteLine ("Goodboy points loaded.");
                    Console.WriteLine ("");
                });

                while (true)
                {
                    string message = Console.ReadLine ();
                    string[] commands = message.Split (' ');

                    boyBot.ParseConsoleCommands (commands);

                    switch (commands[0])
                    {
                        case "test":
                            Console.WriteLine (client.Servers.First().UserCount);
                            break;
                        case "setgame":
                            client.SetGame (new Discord.Game (commands[1].Replace('-',' ')));
                            break;
                        default:
                            break;
                    }
                }
            });
        }
    }
}
