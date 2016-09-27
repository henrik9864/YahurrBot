using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            client.ExecuteAndWait (async () =>
            {
                Console.WriteLine ("Connect to discord...");

                await client.Connect ("MjI4NDYzNTEyMzQ1NzcyMDMy.CsV8lg.pDTRN0u3TP0X19On5jiuRR1TBd0", TokenType.Bot);

                Console.WriteLine ("Bot is connected with key: MjI4NDYzNTEyMzQ1NzcyMDMy.CsV8lg.pDTRN0u3TP0X19On5jiuRR1TBd0.");
                Console.WriteLine ("");

                Console.WriteLine ("Loading goodboy points...");

                await Task.Run (() => { boyBot.LoadPoints (); });

                Console.WriteLine ("Goodboy points loaded.");
                Console.WriteLine ("");

                Console.WriteLine ("Loading modules...");

                await Task.Run (() =>
                {
                    boyBot = new BoyPoints (client);
                });

                Console.WriteLine ("Modules loaded.");

                while (true)
                {
                    string message = Console.ReadLine ();
                    string[] commands = message.Split (' ');

                    boyBot.ParseConsoleCommands (commands);
                }
            });

        }
    }

    class BoyStatus
    {
        [JsonIgnore]
        public User user;
        public string userName;

        int boyPoints;
        [JsonProperty ("points")]
        public int points
        {
            get
            {
                return boyPoints;
            }
        }

        int left = 3;
        [JsonProperty ("toSend")]
        public int toSend
        {
            get
            {
                return left;
            }
        }

        public BoyStatus ( User user )
        {
            this.user = user;
            userName = user.Name;
        }

        public BoyStatus ( Server server, string userName, int points, int toSend )
        {
            boyPoints = points;
            left = toSend;
            this.userName = userName;

            user = server.FindUsers (userName).First ();
        }

        public void UpdateUser ( Server server )
        {
            user = server.FindUsers (userName).First ();
        }

        public void Update ()
        {
            if (boyPoints > 0)
            {
                Role role = user.Server.FindRoles ("Good boy").First ();
                Console.WriteLine (role.Name);
                user.AddRoles (role);
                //user.RemoveRoles (user.Server.FindRoles ("Bad boy").First ());
            }
            else if (boyPoints < 0)
            {
                user.AddRoles (user.Server.FindRoles ("Bad boy").First ());
                //user.RemoveRoles (user.Server.FindRoles ("Good boy").First ());
            }
        }

        public void AddPoint ()
        {
            if (left > 0)
            {
                boyPoints++;
            }
        }

        public void RemovePoint ()
        {
            if (left > 0)
            {
                boyPoints--;
            }
        }

        public void SpendToSend ()
        {
            left--;
        }
    }
}
