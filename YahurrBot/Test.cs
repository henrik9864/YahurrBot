using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace YahurrBot
{
    class Test : Module
    {
        public override void Load ( DiscordClient client )
        {
            Test1 test = new Test1 ();
            test.tesst = 2;

            Save (test, "points", "Henrik", true);

            ExitProgram (new object (), new EventArgs ());

            Test1 points = Load<Test1> ("points", "Henrik");

            Console.WriteLine (points.test.mjesi.mjes3);
        }
    }

    class Test1
    {
        public int tesst = 1;
        public Test2 test = new Test2 ();
    }

    class Test2
    {
        public int mjes = 10;
        public Test3 mjesi = new Test3 ();
    }

    class Test3
    {
        public int mjes3 = 20;
    }
}
