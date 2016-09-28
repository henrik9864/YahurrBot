using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace YahurrBot
{
    class GameCounter
    {
        List<Profile> profiles = new List<Profile> ();
        DiscordClient client;

        public GameCounter ( DiscordClient client )
        {
            this.client = client;
        }

        public void ProfileUpdate ( UserUpdatedEventArgs profile )
        {
            Profile user = FindProfile (profile.After.Name);

            user.FindGame (profile.After.CurrentGame.ToString());
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
                default:
                    break;
            }
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

        bool playingGame;
        public bool isPlaying
        {
            get
            {
                return playingGame;
            }
        }

        public DateTime session
        {
            get
            {
                return DateTime.Now.Subtract (gameTime);
            }
        }

        public DateTime time;

        public Game ( string name )
        {
            gameName = name;
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
            gameTime.Add (span);
        }
    }
}
