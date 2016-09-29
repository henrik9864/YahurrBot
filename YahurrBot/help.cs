using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YahurrBot
{
    class Help
    {
        List<string> list = new List<string>() {
            "!help (page) - Shows this list",
            " Goodboy/Badboy - Gives a goodboy or badboy point",
            "!8ball - It's an eightball, what more is there to say?",
            "!Test - Dette er en test faggots",
            "!Placeholder - en placeholder command",
            "!siste test - jeg lover"
        };

        public void help(string[] commands, Discord.MessageEventArgs e)
        {
            if(commands[0] == "!help")
            {
                list.Sort();
                //sidenummer = page(commands[1])
                string test = "``` Help - page " + (page(commands[1]) + 1) + " of " + Math.Ceiling((float)list.Count / 5) + Environment.NewLine;
                test = test + "-------------------------------------------------------" + Environment.NewLine;

                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        var x = list[page(commands[1]) * 5 + i];
                    }
                    catch
                    {
                        break;
                    }

                    test = test + list[page(commands[1])*5 + i] + Environment.NewLine;
                }

                test = test + "-------------------------------------------------------```";
                e.Channel.SendMessage(test);
                
            }
        }
    
    public int page(string commands)
        {
            int number;
            if (Int32.TryParse(commands, out number))
            {
                if(number > (list.Count - 1))
                {
                    return (list.Count - 1);
                }
                else
                {
                    return (number - 1);
                }
            }
            else
            {
                return 0;
            }

        }
    }
}
