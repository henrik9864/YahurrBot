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

        public void ProfileUpdate ( ProfileUpdatedEventArgs profile )
        {
            Profile user = FindProfile (profile.After.Name);

            Console.WriteLine (profile.After.CurrentGame);

            //user.FindGame (profile.After.CurrentGame.GetValueOrDefault ());
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

        float gameHours;
        public float hours
        {
            get
            {
                return gameHours;
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

        public DateTime time;
        public float session;

        public Game ( string name )
        {
            gameName = name;
        }
    }
}
