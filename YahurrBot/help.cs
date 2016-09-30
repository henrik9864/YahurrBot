using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YahurrBot
{
    class Help
    {
        List<string> list = new List<string>();

        public void startUp()
        {
            addHelp("!help (page) - Shows this list");
            list.Sort();
        }    

        public void help(string[] commands, Discord.MessageEventArgs e)
        {
            if(commands[0] == "!help")
            {
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
    
    private int page(string commands)
        {
            int number;
            if (Int32.TryParse(commands, out number))
            {
                number = Math.Abs(number);
                if(number > (Math.Ceiling((float)list.Count / 5) - 1))
                {
                    return ((int)list.Count / 5);
                }
                else
                {
                    return ((int)number/5);
                }
            }
            else
            {
                return 0;
            }

        }

    public void addHelp(string description)
        {
            list.Add(description);
            list.Sort();
        }
    }
}
