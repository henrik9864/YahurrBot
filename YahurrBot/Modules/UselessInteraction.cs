using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

namespace YahurrBot.Modules
{
    class UselessInteraction : Module
    {
        public override void Load(DiscordClient client)
        {
            Help.addHelp("!8ball <question> ?", "Gives you an answer to your yes/no-question");
        }
        public override void ParseCommands(String[] command, MessageEventArgs e)
        {
            switch (command[0])
            {
                case "hei":
                    if (command.Length == 2 || command[1] == "yahurr" || command[1] == "alle")
                    {
                        e.Channel.SendMessage("Hei, " + e.User.Mention + "!");
                    }
                    break;
                case "!8ball":
                    if (command.Length >= 3 && command[command.Length - 2][command[command.Length - 2].Length - 1] == '?')
                    {
                        String answer = eightBall();
                        e.Channel.SendMessage(answer);
                    }

                    break;
                case "!role":
                    IEnumerable<User> player = e.Server.FindUsers(command[1]);
                    if (command.Length >= 2 && player.Count() >= 1)
                    {
                        e.Channel.SendMessage("Yey!");
                    }
                    break;
                case "!snowboard":
                    e.Channel.SendMessage("Did you mean !showboard?");
                    break;
                case "!fun":
                    e.Channel.SendMessage("https://reddit.com/r/clopclop");
                    break;
                case "wake":
                    if(command[0] == "wake" && command[1] == "me" && command[2] == "up")
                    {
                        e.Channel.SendMessage("Can't wake up");
                    }
                    //e.Channel.SendMessage(debug("inside"));
                    break;
                default:
                    break;
            }

            if(find(command))
            {
                e.Channel.SendMessage("I heard my name. What can I do for you?");
            }
        }

        String eightBall()
        {
            int rand;

            Random random = new Random();

            rand = random.Next(0, 100);
            String[] answer = new String[] { "That would be true", "That would equate to false", "No", "Yes", "You are right", "You are wrong", "Simple minds would believe that to be true", "I am " + rand + "% sure about that" };

            rand = random.Next(0, 8);
            return answer[rand];
        }

        String debug(String arg)
        {
            return "It works";
        }

        bool find(String[] arr)
        {
            foreach(String i in arr)
            {
                if (i == "yahurrbot") return true;
            }
            return false;
        }
    }
}
