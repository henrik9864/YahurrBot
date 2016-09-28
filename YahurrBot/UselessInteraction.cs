using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace YahurrBot
{
    class UselessInteraction
    {
        public void ParseCommands(string[] command, MessageEventArgs e)
        {
            switch (command[0])
            {
                case "hei":
                    if (command.Length == 2 || command[1] == "yahurr" || command[1] == "alle")
                    {
                        e.Channel.SendMessage("Hei, " + e.User.Mention + "!");
                        //int messageLength = command.Length;  e.Channel.SendMessage(messageLength.ToString());
                    }
                    break;
                case null:
                    break;
                default:
                    break;
            }
        }
    }
}
