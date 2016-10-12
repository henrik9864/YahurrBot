using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YahurrBot
{
    abstract class Module
    {
        public static string saveDir = Directory.GetCurrentDirectory ();

        static List<Module> loadedModules = new List<Module> (); // List of all currently loaded modules.

        // Finds all classes that extends Modules and creates and instance of each of them.
        // Then it calls the load function
        public static int LoadModules ( Discord.DiscordClient client )
        {
            IEnumerable<Type> types = from t in Assembly.GetExecutingAssembly ().GetTypes () where t.IsClass && typeof (Module).IsAssignableFrom (t) && t.IsAbstract == false select t;

            LoadFiles ();

            foreach (Type type in types)
            {
                Module module = (Module)Activator.CreateInstance (type);
                loadedModules.Add (module);

                module.Load (client);
            }

            return types.Count ();
        }

        public static void ExitProgram ( object s, EventArgs e )
        {
            SaveFiles ();
            foreach (Module module in loadedModules)
            {
                module.OnExit ();
            }
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

        public static void JoinedUser ( Discord.UserEventArgs e )
        {
            foreach (Module module in loadedModules)
            {
                module.UserJoined (e);
            }
        }

        static void LoadFiles ()
        {
            JArray j = (JArray)JsonConvert.DeserializeObject (File.ReadAllText (saveDir + "/Files/Modules.txt", System.Text.Encoding.UTF8));
            List<DataUser> newUsers = new List<DataUser> ();

            for (int i = 0; i < j.Count; i++)
            {
                string userName = (string)j[i]["name"];
                JArray jsonData = (JArray)j[i]["data"];

                DataUser user = new DataUser (userName);

                for (int a = 0; a < jsonData.Count; a++)
                {
                    Type type = Type.GetType ((string)jsonData[a]["typeName"]);
                    string name = (string)jsonData[a]["name"];
                    JToken item = jsonData[a]["item"];

                    object data = Activator.CreateInstance (type, new object[] { });
                    foreach (JProperty prop in item)
                    {
                        FieldInfo field = data.GetType ().GetField (prop.Name);
                        if (field != null && field.GetType().IsClass == false)
                        {
                            field.SetValue (data, Convert.ChangeType ((string)prop.Value, field.FieldType));
                        }
                    }

                    user.AddData (new Obj (data, name, type), false);
                }

                newUsers.Add (user);
            }

            users = newUsers;
        }

        static void SaveFiles ()
        {
            string json = JsonConvert.SerializeObject (users.ToArray (), Formatting.None);

            File.WriteAllText (saveDir + "/Files/Modules.txt", json, System.Text.Encoding.UTF8);
        }

        /// <summary>
        /// Called when the module is loaded. (Once)
        /// </summary>
        /// <param name="client">Client this module is using.</param>
        public virtual void Load ( Discord.DiscordClient client )
        {

        }

        /// <summary>
        /// Called just before the program exits.
        /// </summary>
        public virtual void OnExit ()
        {

        }

        /// <summary>
        /// Called when a command is typed in the console.
        /// </summary>
        /// <param name="commands">Command pluss each parameter separeted by a space.</param>
        public virtual void ParseConsoleCommands ( string[] commands )
        {

        }

        /// <summary>
        /// Called when a user types a command in the discord chat.
        /// </summary>
        /// <param name="commands">Command pluss each parameter separeted by a space.</param>
        /// <param name="e">Discords MessageEvent.</param>
        public virtual void ParseCommands ( string[] commands, Discord.MessageEventArgs e )
        {

        }

        /// <summary>
        /// Called when a users profile has been updated. (GameChanged, NameCHange, ect...)
        /// </summary>
        /// <param name="e">Dicords UserUpdated event.</param>
        public virtual void ProfileUpdate ( Discord.UserUpdatedEventArgs e )
        {

        }

        /// <summary>
        /// Called when a user joined the server.
        /// </summary>
        /// <param name="e">Discords UserArgs event.</param>
        public virtual void UserJoined ( Discord.UserEventArgs e )
        {

        }

        static List<DataUser> users = new List<DataUser> ();

        public void Save<T> ( T toSave, string name, string userName, bool ovveride ) where T : class
        {
            Obj item = new Obj (toSave, name, typeof (T));
            DataUser user = FindUser (userName);

            if (!user.AddData (item, ovveride))
            {
                Console.WriteLine ("Warning: unable to save '" + name + "' name taken.");
            }

            SaveFiles ();
        }

        public T Load<T> ( string name, string userName ) where T : class
        {
            DataUser user = FindUser (userName);
            Obj obj = user.FindData (name);

            T dest = (T)obj.item;
            if (dest == null)
            {
                Console.WriteLine ("Warning: unable to load '" + name + "' from '" + userName + "', wrong dest type.");
                return null;
            }
            return dest;
        }

        DataUser FindUser ( string userName )
        {
            DataUser user = users.Find (a => { return a.name == userName; });

            if (user == null)
            {
                user = new DataUser (userName);
                users.Add (user);
            }

            return user;
        }
    }

    class DataUser
    {
        string userName;
        public string name
        {
            get
            {
                return userName;
            }
        }

        [Newtonsoft.Json.JsonProperty ("data")]
        List<Obj> data = new List<Obj> ();

        public DataUser ( string name )
        {
            userName = name;
        }

        public bool AddData ( Obj obj, bool ovveride )
        {
            Obj found = data.Find (a => { return a.name == obj.name; });
            if (found == null)
            {
                data.Add (obj);
                return true;
            }
            else if (ovveride)
            {
                data.Remove (found);
                data.Add (obj);
            }

            return false;
        }

        public Obj FindData ( string name )
        {
            return data.Find (a => { return a.name == name; });
        }
    }

    class Obj
    {
        object toSave;
        public object item
        {
            get
            {
                return toSave;
            }
        }

        string objTypeName;
        public string typeName
        {
            get
            {
                return objTypeName;
            }
        }

        string objName;
        public string name
        {
            get
            {
                return objName;
            }
        }

        public Obj ( object item, string name, System.Type type )
        {
            toSave = item;
            objName = name;
            objTypeName = type.AssemblyQualifiedName;
        }
    }
}
