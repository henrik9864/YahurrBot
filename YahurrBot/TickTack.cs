using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void ParseCommands(string[] commands, Discord.MessageEventArgs e )
        {
            Console.WriteLine (commands[0]);

            switch (commands[0])
            {
                case "tictac":
                    DrawBoard (e.Channel);

                    break;
                case "set":
                    int index = int.Parse (commands[1])*2 + 1;
                    string line = board[index];
                    line = line.Substring (0, int.Parse(commands[2])*4+3) + commands[3] + line.Substring (int.Parse (commands[2])*4 + 4);

                    board[index] = line;

                    DrawBoard (e.Channel);
                    break;
                default:
                    break;
            }
        }

        void DrawBoard (Discord.Channel channel)
        {
            string toSend = "```";

            foreach (string stt in board)
            {
                toSend += stt + Environment.NewLine;
            }

            toSend += "```";

            channel.SendMessage (toSend);
        }

    }
}