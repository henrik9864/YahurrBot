using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YahurrBot
{
    class GameCounter
    {
        string path = Directory.GetCurrentDirectory ();
        List<Profile> profiles = new List<Profile> ();

        public void ProfileUpdate ( UserUpdatedEventArgs profile )
        {
            if (profile.After.CurrentGame.HasValue)
            {
                Game gameAfter = FindProfile (profile.After.Name).FindGame (profile.After.CurrentGame.Value.Name);
                gameAfter.StartPlaying ();
            }

            if (profile.Before.CurrentGame.HasValue)
            {
                Game gameBefore = FindProfile (profile.Before.Name).FindGame (profile.Before.CurrentGame.Value.Name);
                gameBefore.StopPlaying ();
            }

            SaveTime ();
        }

        public void ParseCommands ( string[] commands, MessageEventArgs e )
        {
            switch (commands[0])
            {
                case "!time":
                    Profile profile = FindProfile (e.User.Name);
                    string toSay = "```";

                    if (commands[1] != "")
                    {
                        User user = e.Server.FindUsers (commands[1]).First ();
                        profile = FindProfile (user.Name);
                        toSay = user.Mention + Environment.NewLine + "```";
                    }

                    foreach (Game game in profile.games)
                    {
                        toSay += game.name + " : " + game.timePlayed + Environment.NewLine;
                    }

                    toSay += " ```";
                    e.Channel.SendMessage (toSay);

                    break;
                default:
                    break;
            }
        }

        public void ParseConsoleCommands ( string[] commdands )
        {
            switch (commdands[0])
            {
                case "playgame":
                    Profile user = FindProfile (commdands[1]);

                    Game game = user.FindGame (commdands[2]);
                    game.StartPlaying ();
                    break;
                case "stopgame":
                    Profile u = FindProfile (commdands[1]);

                    Game g = u.FindGame (commdands[2]);
                    g.StopPlaying ();
                    break;
                case "getgametime":
                    Profile u1 = FindProfile (commdands[1]);

                    Game g1 = u1.FindGame (commdands[2]);
                    Console.WriteLine (g1.timePlayed);
                    break;
                default:
                    break;
            }
        }

        public void SaveTime ()
        {
            string json = JsonConvert.SerializeObject (profiles.ToArray (), Formatting.None);

            File.WriteAllText (path + "/Files/GameCounter.txt", json, System.Text.Encoding.UTF8);
        }

        public void LoadPoints ()
        {
            Help.addHelp("!time", "Gives you a list of games you've played and amount of hours");

            JArray j = (JArray)JsonConvert.DeserializeObject (File.ReadAllText (path + "/Files/GameCounter.txt", System.Text.Encoding.UTF8));
            List<Profile> newProfiles = new List<Profile> ();

            for (int i = 0; i < j.Count; i++)
            {
                string userName = (string)j[i]["userName"];

                JArray foundGames = (JArray)j[i]["games"];
                List<Game> newGames = new List<Game> ();
                for (int a = 0; a < foundGames.Count; a++)
                {
                    string gameName = (string)foundGames[a]["name"];
                    TimeSpan time = TimeSpan.Parse ((string)foundGames[a]["timePlayed"]);

                    newGames.Add (new Game (gameName, time));
                }

                newProfiles.Add (new Profile (userName, newGames));
            }

            profiles = newProfiles;
        }

        Profile FindProfile ( string name )
        {
            Profile profile = profiles.Find (a => { return a.userName == name; });

            if (profile == null)
            {
                profile = new Profile (name);
                profiles.Add (profile);
            }

            return profile;
        }
    }

    class Profile
    {
        string user;
        public string userName
        {
            get
            {
                return user;
            }
        }

        public List<Game> games = new List<Game> ();

        public Profile ( string name )
        {
            user = name;
        }

        public Profile ( string name, List<Game> games )
        {
            user = name;
            this.games = games;
        }

        public Game FindGame ( string name )
        {
            Game game = games.Find (a => { return a.name == name; });

            if (game == null)
            {
                game = new Game (name);
                games.Add (game);
            }

            return game;
        }
    }

    class Game
    {
        string gameName;
        public string name
        {
            get
            {
                return gameName;
            }
        }

        TimeSpan gameTime;
        public TimeSpan timePlayed
        {
            get
            {
                return gameTime;
            }
        }
        [JsonIgnore]
        public DateTime session
        {
            get
            {
                return DateTime.Now.Subtract (gameTime);
            }
        }

        bool playingGame;
        [JsonIgnore]
        public bool isPlaying
        {
            get
            {
                return playingGame;
            }
        }

        DateTime time;

        public Game ( string name )
        {
            gameName = name;
        }

        public Game ( string name, TimeSpan timePlayed )
        {
            gameName = name;
            gameTime = timePlayed;
        }

        public void StartPlaying ()
        {
            playingGame = true;
            time = DateTime.Now;
        }

        public void StopPlaying ()
        {
            playingGame = false;

            TimeSpan span = DateTime.Now.Subtract (time);
            gameTime = gameTime.Add (span);
        }
    }
}
