using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YahurrBot
{
    abstract class Module
    {
        static List<Module> loadedModules = new List<Module> ();

        public static int LoadModules ( Discord.DiscordClient client )
        {
            IEnumerable<Type> types = from t in Assembly.GetExecutingAssembly ().GetTypes () where t.IsClass && typeof (Module).IsAssignableFrom (t) && t.IsAbstract == false select t;

            foreach (Type type in types)
            {
                Module module = (Module)Activator.CreateInstance (type);
                loadedModules.Add (module);

                module.Load (client);
            }

            return types.Count ();
        }

        public static void ConsoleCommand ( string[] commands )
        {
            foreach (Module module in loadedModules)
            {
                module.ParseConsoleCommands (commands);
            }
        }

        public static void Command ( string[] commands, Discord.MessageEventArgs e )
        {
            foreach (Module module in loadedModules)
            {
                module.ParseCommands (commands, e);
            }
        }

        public static void UpdateProfile ( Discord.UserUpdatedEventArgs e )
        {
            foreach (Module module in loadedModules)
            {
                module.ProfileUpdate (e);
            }
        }

        public virtual void Load ( Discord.DiscordClient client )
        {

        }

        public virtual void ParseConsoleCommands ( string[] commands )
        {

        }

        public virtual void ParseCommands ( string[] commands, Discord.MessageEventArgs e )
        {

        }

        public virtual void ProfileUpdate ( Discord.UserUpdatedEventArgs e )
        {

        }
    }
}
