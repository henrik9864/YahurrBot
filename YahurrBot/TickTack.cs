using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace YahurrBot
{
    class TickTack
    {

        string[] board = new string[]
        {
            "   1   2   3",
            "A    |   |   ",
            "  ---+---+---",
            "B    |   |   ",
            "  ---+---+---",
            "C    |   |   ",
        };

        List<Game> games = new List<Game> ();
        DiscordClient client;

        public TickTack ( DiscordClient client )
        {
            this.client = client;
        }

        public void ParseCommands ( string[] commands, MessageEventArgs e )
        {
            Console.WriteLine (commands[0]);

            switch (commands[0])
            {
                case "!challenge":
                    User challenged = client.Servers.First ().FindUsers (commands[1]).First ();

                    games.Add (new Game (e.User.Name, challenged.Name));
                    e.Channel.SendMessage (challenged.Mention + " has been challenged to TickTackToe by " + e.User.Mention + " type !accept to accept.");

                    break;
                case "!accept":
                    Game game = FindGame (e.User.Name, false);

                    if (game != null)
                    {
                        e.Channel.SendMessage (e.User.Mention + " has accepted the game.");
                        e.Channel.SendMessage (e.User.Mention + " is statring.");
                        e.Channel.SendMessage ("Starting game...");

                        game.board = GenerateBoard (3);
                        game.StartGame ();
                        DrawBoard (e.Channel, game.board);
                    }
                    else
                    {
                        e.Channel.SendMessage ("You have no pending TickTackToe requests.");
                    }
                    break;
                default:
                    Game foundGame = FindGame (e.User.Name, true);

                    if (foundGame != null && commands[1] == "" && commands[0].Length == 2)
                    {
                        char[] charArray = commands[0].ToCharArray ();
                        int y = char.ToUpper (charArray[0]) - 64;

                        string player = foundGame.PlayRound ((int)char.GetNumericValue (commands[0][1]), y, e.User.Name);

                        e.Channel.SendMessage (player + "'s turn.");
                        DrawBoard (e.Channel, foundGame.board);
                    }

                    break;
            }
        }

        void DrawBoard ( Discord.Channel channel, string[] board )
        {
            string toSend = "```";

            foreach (string stt in board)
            {
                toSend += stt + Environment.NewLine;
            }

            toSend += "```";

            channel.SendMessage (toSend);
        }

        string[] GenerateBoard ( int size )
        {
            size--; // Make it 1 based
            string[] newBoard = new string[size * 2 + 2];

            string line1 = "   1 ";

            for (int i = 0; i < size; i++)
            {
                line1 += "  " + (i + 2) + " ";
            }

            newBoard[0] = line1;

            for (int i = 0; i <= size * 2; i++)
            {
                if (i % 2 == 0)
                {
                    newBoard[i + 1] = Number2String ((int)Math.Floor ((decimal)(i / 2) + 1), true) + " " + GenerateLine (size, false);
                }
                else
                {
                    newBoard[i + 1] = "  " + GenerateLine (size, true);
                }
            }

            return newBoard;
        }

        string GenerateLine ( int length, bool border )
        {
            string line = "   ";

            if (border)
            {
                line = "---";
            }

            for (int i = 0; i < length; i++)
            {
                if (border)
                {
                    line += "+---";
                }
                else
                {
                    line += "|   ";
                }
            }

            return line;
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
            List<string> symbols = new List<string> () { "X", "O", "/", "|", "-" };
            public bool accepted = false;

            int turn = 0;

            public string[] board;

            public Game ( string challenger, string challenged )
            {
                players.Add (challenged);
                players.Add (challenger);
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

            public string PlayRound ( int x, int y, string player )
            {
                if (players[turn] == player)
                {
                    y--;
                    x--;
                    string line = board[y * 2 + 1];
                    line = line.Substring (0, x * 4 + 3) + symbols[turn] + line.Substring (x * 4 + 4);

                    board[y * 2 + 1] = line;

                    turn++;
                    if (turn >= players.Count)
                    {
                        turn = 0;
                    }
                    return players[turn];
                }

                return players[turn];
            }
        }
    }
}