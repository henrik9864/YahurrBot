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

        string path = Directory.GetCurrentDirectory ();
        DiscordClient client;

        List<BoyStatus> users = new List<BoyStatus> ();

        public void Start ()
        {
            client = new DiscordClient ();

            client.MessageReceived += ( s, e ) =>
            {
                if (!e.Message.IsAuthor)
                {
                    string message = e.Message.Text + " ";
                    string[] commdands = message.ToLower ().Split (' ');

                    ParseCommands (commdands, e);
                }
            };

            client.ExecuteAndWait (async () =>
            {
                Console.WriteLine ("Connect to discord...");

                await client.Connect ("MjI4NDYzNTEyMzQ1NzcyMDMy.CsV8lg.pDTRN0u3TP0X19On5jiuRR1TBd0", TokenType.Bot);

                Console.WriteLine ("Bot is connected with key: MjI4NDYzNTEyMzQ1NzcyMDMy.CsV8lg.pDTRN0u3TP0X19On5jiuRR1TBd0.");
                Console.WriteLine ("");

                while (true)
                {
                    string message = Console.ReadLine ();
                    string[] commands = message.Split (' ');

                    switch (commands[0])
                    {
                        case "say":
                            await client.FindServers (commands[1]).First ().FindChannels (commands[2]).First ().SendMessage (commands[3].Replace ('-', ' '));
                            break;
                        case "save":
                            SavePoints ();
                            break;
                        case "load":
                            LoadPoints ();
                            break;
                        case "reset":
                            File.WriteAllText (path + "/Files/Saves.txt", string.Empty);
                            break;
                        default:
                            break;
                    }
                }
            });

        }

        void ParseCommands ( string[] commdands, MessageEventArgs e )
        {
            switch (commdands[0])
            {
                case "goodboy":
                    Goodboy (commdands, e);
                    SavePoints ();
                    break;
                case "badboy":
                    Badboy (commdands, e);
                    SavePoints ();
                    break;
                case "!points":
                    ShowPoints (commdands, e);
                    break;
                default:
                    break;
            }
        }

        void Goodboy ( string[] commdands, MessageEventArgs e )
        {
            IEnumerable<User> clients = e.Channel.FindUsers (commdands[1]);
            User user = null;
            if (client != null)
                user = clients.First ();
            if (user != null && e.Message.User != user)
            {
                BoyStatus boy = FindBoy (e.User);
                if (boy.toSend > 0)
                {
                    FindBoy (user).AddPoint ();
                    boy.SpendToSend ();
                }

                e.Channel.SendMessage (user.Mention + " gained a good boy point.");
            }
        }

        void Badboy ( string[] commdands, MessageEventArgs e )
        {
            IEnumerable<User> clients = e.Channel.FindUsers (commdands[1]);
            User user = null;

            clients = e.Channel.FindUsers (commdands[1]);
            user = null;
            if (client != null)
                user = clients.First ();
            if (user != null && e.Message.User != user)
            {
                BoyStatus boy = FindBoy (e.User);
                if (boy.toSend > 0)
                {
                    FindBoy (user).RemovePoint ();
                    boy.SpendToSend ();
                }

                e.Channel.SendMessage (user.Mention + " lost a good boy point.");
            }
        }

        void ShowPoints ( string[] commdands, MessageEventArgs e )
        {
            IEnumerable<User> clients = e.Channel.FindUsers (commdands[1]);
            User user = e.User;
            BoyStatus status = FindBoy (user);

            switch (status.points)
            {
                case 1:
                    e.Channel.SendMessage (user.Mention + " has " + status.points + " point.");
                    break;
                default:
                    e.Channel.SendMessage (user.Mention + " has " + status.points + " points.");
                    break;
            }
        }

        void SavePoints ()
        {
            string json = JsonConvert.SerializeObject (users.ToArray (), Formatting.None);

            File.WriteAllText (path + "/Files/Saves.txt", json, System.Text.Encoding.UTF8);

        }

        void LoadPoints ()
        {
            JArray j = (JArray)JsonConvert.DeserializeObject (File.ReadAllText (path + "/Files/Saves.txt", System.Text.Encoding.UTF8));

            List<BoyStatus> newUsers = new List<BoyStatus> ();

            for (int i = 0; i < j.Count; i++)
            {
                string userName = (string)j[i]["userName"];
                int points = int.Parse ((string)j[i]["points"]);
                int toSend = int.Parse ((string)j[i]["toSend"]);

                newUsers.Add (new BoyStatus (client.Servers.First (), userName, points, toSend));
            }

            users = newUsers;
        }

        BoyStatus FindBoy ( User user )
        {
            BoyStatus status = users.Find (a => { return a.user == user; });

            if (status == null)
            {
                status = new BoyStatus (user);
                users.Add (status);
            }

            return status;
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
