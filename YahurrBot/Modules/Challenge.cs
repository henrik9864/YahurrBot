﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;
using YahurrBot.Games;

namespace YahurrBot.Modules
{
    class Challenge : Module
    {
        static List<GameInfo> activeGames = new List<GameInfo> ();

        //DONT JUDGE OKAY!
        static void StopGame(GameInfo game )
        {
            activeGames.Remove (game);
        }

        public override void ParseCommands ( string[] commands, MessageEventArgs e )
        {
            switch (commands[0])
            {
                case "!challenge":
                    GameInfo newGame;
                    Type gameType = Type.GetType ("YahurrBot.Games." + commands[1] + ", YahurrBot", false, true);

                    if (gameType != null && typeof (Game).IsAssignableFrom (gameType))
                    {
                        newGame = new GameInfo (e.User, gameType, e.Channel);
                    }
                    else
                    {
                        e.Channel.SendMessage ("Invalid game selected.");
                        break;
                    }

                    if (commands[2] == "")
                    {
                        activeGames.Add (newGame);

                        e.Channel.SendMessage (e.User.Mention + " has created a challenge type !Join " + e.User.Name + " to join.");
                        e.Channel.SendMessage ("type !Start to start when all players have joined.");
                    }
                    else
                    {
                        User user = e.Channel.FindUsers (commands[2]).First ();
                        User challenged = FindPlayer (e.Server, commands[2]);

                        newGame.Whitelist (user);
                        activeGames.Add (newGame);

                        e.Channel.SendMessage (e.User.Mention + " has challenged " + challenged.Mention + " type !Accept to join.");
                    }
                    break;
                case "!join":
                    User player = FindPlayer (e.Server, commands[1]);
                    GameInfo game = FindGame (player, false, false);

                    if (game != null && game.JoinGame (e.User))
                    {
                        e.Channel.SendMessage ("You have joined " + game.creator.Mention + "'s game");
                    }
                    else
                    {
                        e.Channel.SendMessage ("Game not found.");
                    }
                    break;
                case "!accept":
                    game = FindGame (e.User, true, false);

                    if (game != null && game.JoinGame (e.User))
                    {
                        e.Channel.SendMessage ("Game accepted.");
                        e.Channel.SendMessage ("Starting game...");

                        game.StartGame ();
                    }
                    else
                    {
                        e.Channel.SendMessage ("Game not found.");
                    }
                    break;
                case "!start":
                    game = FindGame (e.User, false, true);

                    if (game != null)
                    {
                        e.Channel.SendMessage ("Starting game...");

                        game.StartGame ();
                    }
                    else
                    {
                        e.Channel.SendMessage ("Game not found.");
                    }
                    break;
                case "!stop":
                    game = FindGame (e.User, false, true);
                    activeGames.Remove (game);
                    break;
                case "!listgames":
                    
                    break;
                default:
                    if (commands[0].Substring (0, 1) == "#")
                    {
                        game = FindGame (e.User, false, false);
                        commands[0] = commands[0].Substring (1);

                        game.PlayRound (e.User, commands);
                    }
                    break;
            }
        }

        GameInfo FindGame ( User user, bool whitelisted, bool creator )
        {
            if (whitelisted)
            {
                return activeGames.Find (a => { return a.Whitelisted (user); });
            }
            else
            {
                return activeGames.Find (a => { return a.creator.Name.ToLower () == user.Name.ToLower () || (!creator && a.Joined (user)); });
            }
        }

        GameInfo FindGame ( Game game )
        {
            return activeGames.Find (a => { return a.game == game; });
        }

        class GameInfo
        {
            User gameCreator;
            public User creator
            {
                get
                {
                    return gameCreator;
                }
            }

            Game gameGame;
            public Game game
            {
                get
                {
                    return gameGame;
                }
            }

            List<User> joined = new List<User> ();

            Channel channel;
            Message latestMessage;
            Type gameType;
            int playerPlaying;
            bool stopped;

            public bool hasWhitelist;
            List<User> whitelit = new List<User> ();

            public GameInfo ( User creator, Type type, Channel channel )
            {
                gameCreator = creator;
                gameType = type;

                gameGame = (Game)Activator.CreateInstance (gameType);
                game.ConfigureGame ();

                this.channel = channel;
            }

            public void Whitelist ( User user )
            {
                whitelit.Add (user);
                hasWhitelist = true;
            }

            public bool Whitelisted ( User user )
            {
                return whitelit.Find (a => { return a.Equals (user); }) != null;
            }

            public bool Joined ( User user )
            {
                return joined.Find (a => { return a.Equals (user); }) != null;
            }

            public bool JoinGame ( User user )
            {
                User found = whitelit.Find (a => { return a == user; });
                if (hasWhitelist && found == null && joined.Count <= game.settings.maxPlayers)
                {
                    return false;
                }
                joined.Add (user);
                return true;
            }

            public void StartGame ()
            {
                string board = game.StartGame (creator, joined);
                channel.SendMessage (creator.NicknameMention + " is starting.");
                Task.Run (async () =>
                {
                    latestMessage = await channel.SendMessage (board);
                });
            }

            public void PlayRound ( User playing, string[] arguments )
            {
                if (GetUser (playerPlaying) == playing && !stopped)
                {
                    string toDraw = game.PlayRound (playerPlaying, arguments);
                    bool hasWon = game.HasWon ();

                    if (toDraw == "")
                    {
                        channel.SendMessage ("Invalid move, please repeat.");
                        return;
                    }

                    if (latestMessage != null)
                    {
                        channel.DeleteMessages (new Message[] { latestMessage });
                    }

                    if (hasWon)
                    {
                        channel.SendMessage (toDraw + Environment.NewLine + GetUser (playerPlaying).Mention + " has won the game!");

                        Challenge.StopGame (this);
                        stopped = true;
                        return;
                    }
                    else
                    {
                        Task.Run (async () =>
                        {
                            latestMessage = await channel.SendMessage (toDraw + Environment.NewLine + GetUser (playerPlaying).Mention + "'s turn.");
                        });
                    }

                    playerPlaying++;
                    if (playerPlaying >= joined.Count + 1)
                    {
                        playerPlaying = 0;
                    }
                }
                else
                {
                    channel.SendMessage ("It is not your turn!");
                }
            }

            User GetUser ( int id )
            {
                if (id == 0)
                {
                    return creator;
                }
                else
                {
                    return joined[id - 1];
                }
            }
        }
    }

    class Game
    {
        private Settings gameSettings = new Settings ();
        public Settings settings
        {
            get
            {
                return gameSettings;
            }
        }

        public virtual void ConfigureGame ()
        {

        }

        public virtual string StartGame ( User creator, List<User> joined )
        {
            return "";
        }

        public virtual string PlayRound ( int player, string[] arguments )
        {
            return "";
        }

        public virtual bool HasWon ()
        {
            return true;
        }

        public class Settings
        {
            /// <summary>
            /// Max amount of allowed players. (Excluding creator)
            /// </summary>
            public int maxPlayers;
        }
    }
}
