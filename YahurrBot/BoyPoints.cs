using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Discord;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YahurrBot
{
    class BoyPoints
    {
        string path = Directory.GetCurrentDirectory ();
        List<BoyStatus> users = new List<BoyStatus> ();

        DiscordClient client;

        public BoyPoints ( DiscordClient client )
        {
            this.client = client;
        }

        /// <summary>
        /// Executes command typed in the discord chat.
        /// </summary>
        public void ParseCommands ( string[] commdands, MessageEventArgs e )
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

        /// <summary>
        /// Executes command typed in the console.
        /// </summary>
        public void ParseConsoleCommands ( string[] commdands )
        {
            switch (commdands[0])
            {
                case "say":
                    client.FindServers (commdands[1]).First ().FindChannels (commdands[2]).First ().SendMessage (commdands[3].Replace ('-', ' '));
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

        public void SavePoints ()
        {
            string json = JsonConvert.SerializeObject (users.ToArray (), Formatting.None);

            File.WriteAllText (path + "/Files/Saves.txt", json, System.Text.Encoding.UTF8);
        }

        public void LoadPoints ()
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

            IEnumerable<User> found = server.FindUsers (userName);

            if (found.Count () > 0)
            {
                user = found.First ();
            }
            else
                Console.WriteLine ("Warning unable to find user: " + userName);
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
