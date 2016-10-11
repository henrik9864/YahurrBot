using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace YahurrBot
{
    class TickTack : Module
    {
        List<Game> games = new List<Game> ();
        DiscordClient client;

        public override void Load ( DiscordClient client )
        {
            this.client = client;
            Help.addHelp ("!challenge", "Challenge a player to a TickTackToe battle to the death");
        }

        public override void ParseCommands ( string[] commands, MessageEventArgs e )
        {
            switch (commands[0])
            {
                case "!challenge":
                    User challenged = client.Servers.First ().FindUsers (commands[1]).First ();

                    games.Add (new Game (e.User.Name, challenged.Name, e.Channel));
                    e.Channel.SendMessage (challenged.Mention + " has been challenged to TickTackToe by " + e.User.Mention + " type !accept to accept.");

                    break;
                case "!accept":
                    Game game = FindGame (e.User.Name, false);

                    if (game != null)
                    {
                        e.Channel.SendMessage (e.User.Mention + " has accepted the game.");
                        e.Channel.SendMessage (e.User.Mention + " is starting.");
                        e.Channel.SendMessage ("Starting game...");

                        game.board = MakeBoard (3);
                        game.StartGame ();
                        ShowBard (game.board, e.Channel);
                    }
                    else
                    {
                        e.Channel.SendMessage ("You have no pending TickTackToe requests.");
                    }
                    break;
                case "!stop":
                    game = FindGame (e.User.Name, false);

                    if (game != null)
                    {
                        games.Remove (game);
                        e.Channel.SendMessage ("Game canceled.");
                    }
                    else
                    {
                        e.Channel.SendMessage ("You dont have any pending TickTackToe games.");
                    }
                    break;
                default:
                    Game foundGame = FindGame (e.User.Name, true);

                    if (foundGame != null && commands[1] == "" && commands[0].Length == 2)
                    {
                        char[] charArray = commands[0].ToCharArray ();
                        int y = char.ToUpper (charArray[0]) - 64;


                        if (foundGame.PlayRound ((int)char.GetNumericValue (commands[0][1]), y, e.User.Name))
                        {
                            Console.WriteLine ("Won!");
                            games.Remove (foundGame);
                        }
                        else
                        {
                            Console.WriteLine ("Done");
                            ShowBard (foundGame.board, e.Channel);
                        }
                    }

                    break;
            }
        }

        Square[,] MakeBoard ( int size )
        {
            size++;
            Square[,] board = new Square[size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    board[x, y] = new Square ();
                }
            }

            return board;
        }

        void ShowBard ( Square[,] board, Channel channel )
        {
            string toSend = "```  ";

            for (int x = 1; x < board.GetUpperBound (0) + 1; x++)
            {
                toSend += " " + x + "  ";
            }

            toSend += Environment.NewLine;

            for (int x = 0; x < board.GetUpperBound (0) * 2 - 1; x++)
            {
                string line = "  ";
                if (x % 2 == 0)
                {
                    line = Number2String (x / 2 + 1, true) + " ";
                }

                for (int y = 0; y < board.GetUpperBound (1); y++)
                {
                    if (x % 2 == 0)
                    {
                        line += " " + board[x / 2, y].symbol + " |";
                    }
                    else
                    {
                        line += "---+";
                    }
                }

                toSend += line.Substring (0, line.Length - 1) + Environment.NewLine;
            }

            toSend += "```";

            channel.SendMessage (toSend);
        }

        string Number2String ( int number, bool isCaps )
        {
            char c = (char)((isCaps ? 65 : 97) + (number - 1));
            return c.ToString ();
        }

        Game FindGame ( string challenged, bool accepted )
        {
            Game game = games.Find (a => { return a.isPlaying (challenged) && a.accepted == accepted; });
            return game;
        }

        class Game
        {

            List<string> players = new List<string> ();
            List<char> symbols = new List<char> () { 'X', 'O', '/', '|', '-' };
            public Channel channel;
            public bool accepted = false;

            int playerIndex = 0;
            int turn = 0;

            public Square[,] board;

            public Game ( string challenger, string challenged, Channel channel )
            {
                players.Add (challenged);
                players.Add (challenger);

                this.channel = channel;
            }

            public void StartGame ()
            {
                accepted = true;
            }

            public bool isPlaying ( string player )
            {
                if (players.Find (a => { return a == player; }) != null)
                {
                    return true;
                }
                return false;
            }

            public bool PlayRound ( int x, int y, string player )
            {
                x--;
                y--;

                if (players[playerIndex] == player && !board[y, x].taken)
                {
                    board[y, x].Take (symbols[playerIndex]);

                    if (HasWon (y, x, symbols[playerIndex]))
                    {
                        channel.SendMessage (channel.FindUsers (player).First ().Mention + " has won the game!");
                        return true;
                    }

                    playerIndex++;
                    if (playerIndex >= players.Count)
                    {
                        playerIndex = 0;
                    }
                }

                if (turn >= board.GetUpperBound (0) * board.GetUpperBound (1) - 1)
                {
                    channel.SendMessage ("The game is a tie!");
                    return true;
                }
                else
                {
                    turn++;
                }

                channel.SendMessage (players[playerIndex] + "'s turn.");
                return false;
            }

            public bool HasWon ( int x, int y, char symbol )
            {
                int winCountX = 0;
                for (int i = 0; i < board.GetUpperBound (1); i++)
                {
                    if (board[x, i].symbol == symbol)
                    {
                        winCountX++;
                    }
                }

                int winCountY = 0;
                for (int i = 0; i < board.GetUpperBound (0); i++)
                {
                    if (board[i, y].symbol == symbol)
                    {
                        winCountY++;
                    }
                }

                for (int ix = 0; ix < board.GetUpperBound (0); ix++)
                {
                    if (board.GetUpperBound (1) - ix >= 3)
                    {
                        int spree = 0;
                        for (int iy = 0; iy < board.GetUpperBound (1) - ix; iy++)
                        {
                            if (board[ix + iy, iy].symbol == symbol)
                            {
                                spree++;
                            }
                            else
                            {
                                spree--;
                            }
                        }

                        if (spree >= 3)
                        {
                            return true;
                        }
                    }
                }

                if (x == 1 && y == 1)
                {
                    if (board[x + 1, y + 1].symbol == symbol && board[x - 1, y - 1].symbol == symbol)
                    {
                        return true;
                    }

                    if (board[x - 1, y + 1].symbol == symbol && board[x + 1, y - 1].symbol == symbol)
                    {
                        return true;
                    }
                }

                return winCountX >= 3 || winCountY >= 3;
            }
        }

        class Square
        {
            char squareSymbol = ' ';
            public char symbol
            {
                get
                {
                    return squareSymbol;
                }
            }

            bool sqareTaken;
            public bool taken
            {
                get
                {
                    return sqareTaken;
                }
            }

            public void Take ( char symbol )
            {
                squareSymbol = symbol;
                sqareTaken = true;
            }
        }
    }
}