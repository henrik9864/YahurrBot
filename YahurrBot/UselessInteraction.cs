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
                        //Debug message to find length of command
                        //int messageLength = command.Length;  e.Channel.SendMessage(messageLength.ToString());
                    }
                    break;
                case "!8ball":
                    if (command.Length >= 3 && command[command.Length - 2][command[command.Length - 2].Length - 1] == '?')
                    {
                        String answer = eightBall();
                        //Debug message with interesting numbers
                        //e.Channel.SendMessage(answer + " " + command.Length.ToString() + " [" + command[command.Length - 2][command[command.Length - 2].Length - 1] + "]");
                        e.Channel.SendMessage(answer);
                    }

                    break;
                case "!role":
                    IEnumerable<User> player = e.Server.FindUsers(command[1]);
                    if (command.Length >= 2 && player.Count() >= 1)
                    {
                        e.Channel.SendMessage("Yey!");
                        //IEnumerable<Role> role = e.User.Roles.
                    }
                    break;
                case "!snowboard":
                    e.Channel.SendMessage("Did you mean !showboard?");
                    break;
                default:
                    break;
            }
        }

        String eightBall()
        {
            String answer = "";
            int rand;

            Random random = new Random();

            rand = random.Next(0, 8);

            switch (rand)
            {
                case 0:
                    answer = "That would be true";
                    break;
                case 1:
                    answer = "That would equate to false";
                    break;
                case 2:
                    answer = "No";
                    break;
                case 3:
                    answer = "Yes";
                    break;
                case 4:
                    answer = "You are right";
                    break;
                case 5:
                    answer = "You are wrong";
                    break;
                case 6:
                    answer = "Simple minds would believe that to be true";
                    break;
                case 7:
                    rand = random.Next(0, 100);
                    answer = "I am " + rand + "% sure about that";
                    break;
                default:
                    break;
            }

            return answer;
        }
    }
}
