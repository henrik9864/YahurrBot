using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace YahurrBot
{
    class UselessInteractions
    {
        public void ParseCommands(string[] command, MessageEventArgs e)
        {
            switch(command[0])
            {
                case "hei":
                    e.Channel.SendMessage("Hei, " + e.User.Mention + "!");
                    break;
                default:
                    break;
            }
        }
    }
}
