using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Console
{
    public class CommandMenu
    {
        public string Title { get; set; }
        public CommandMenu Parent { get; set; }
        public Dictionary<string, Command> Commands { get; set; }


        public CommandMenu()
        {
            this.Parent = null;
            this.Commands = new Dictionary<string, Command>();
        }

        public CommandMenu(CommandMenu parentMenu)
        {
            this.Parent = parentMenu;
            this.Commands = new Dictionary<string, Command>();
        }

        public static T SelectFromList<T>(IList<string> keys, IList<T> values, string title)
        {
            return SelectFromList(keys, values, title, true);
        }

        public static T SelectFromList<T>(IList<string> keys, IList<T> values, string title, bool clearScreen)
        {
            if (clearScreen)
            {
                ShowHeader();
            }

            ShowTitle(title);

            int index = 1;
            foreach (string key in keys)
            {
                System.Console.WriteLine("{0}. {1}", index, key);
                index++;
            }

            System.Console.WriteLine("\n");
            System.Console.Write("Please enter your selection: ");

            string input = System.Console.ReadLine().Trim();

            if (clearScreen)
            {
                System.Console.Clear();
            }

            int inputInt;

            if (Int32.TryParse(input, out inputInt))
            {
                if (inputInt > 0 && values.Count >= inputInt)
                {
                    return values.ElementAt(inputInt - 1);
                }
            }

            return default(T);
        }

        public void AddCommand(string title, Action action)
        {
            string shortcut = (this.Commands.Count + 1).ToString();
            this.Commands.Add(shortcut, new Command(shortcut, title, action));
        }

        public void ShowTitle()
        {
            if (this.Title != null)
            {
                ShowTitle(this.Title);
            }
        }

        public static void ShowTitle(string title)
        {
            System.Console.WriteLine("--------------- [ {0} ] ----------------------------\n", title);
        }

        public void Show()
        {
            ShowHeader();
            this.ShowTitle();

            foreach (Command command in this.Commands.Values)
            {
                System.Console.WriteLine("{0}. {1}", command.Shortcut, command.Description);
            }

            //if we have a parent menu - add a 'Back' command...
            if (this.Parent != null)
            {
                System.Console.WriteLine("<< Press enter to go Back.");
            }

            System.Console.WriteLine();
            System.Console.Write("Please enter your selection: ");

            string input = System.Console.ReadLine().Trim();
            System.Console.Clear();

            //handle empty input...
            if (input.Length == 0)
            {
                if (this.Parent != null)
                {
                    this.Parent.Show();
                }
                else
                {
                    this.Show();
                }

                return;
            }


            Command enteredCommand = null;
            this.Commands.TryGetValue(input, out enteredCommand);

            if (enteredCommand != null)
            {
                enteredCommand.Execute();
                this.Show();
            }
            else
            {
                this.Show();
            }
        }


        public static void ShowHeader()
        {
            System.Console.Clear();
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine();
            string title = @"
      _   _    ___     ____  __ ____     ____    _____                  _____               _             
     | | | |  / \ \   / /  \/  |___ \   |___ \  |_   _| __ __ _ _ __ __|_   _| __ __ _  ___| | _____ _ __ 
     | |_| | / _ \ \ / /| |\/| | __) |    __) |   | || '__/ _` | '_ ` _ \| || '__/ _` |/ __| |/ / _ \ '__|
     |  _  |/ ___ \ V / | |  | |/ __/    / __/    | || | | (_| | | | | | | || | | (_| | (__|   <  __/ |   
     |_| |_/_/   \_\_/  |_|  |_|_____|  |_____|   |_||_|  \__,_|_| |_| |_|_||_|  \__,_|\___|_|\_\___|_|   
                                                                                                          
";
            System.Console.WriteLine(title);
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine();
        }
    }
}
