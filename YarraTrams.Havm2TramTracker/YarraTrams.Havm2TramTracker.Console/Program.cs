using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Console
{
    public partial class Program
    {
        //console colors
        static ConsoleColor CONSOLE_COLOR_MAIN = ConsoleColor.Yellow;

        //command menus
        static CommandMenu MainMenu = null;
        static CommandMenu ProdSupportMenu = null;
        static CommandMenu TestingMenu = null;
        static CommandMenu AvmRevisionMenu = null;

        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "sidebyside":
                            var x = new TestComparisons.Comparisons();
                            x.RunComparisons();
                            break;
                        default:
                            System.Console.WriteLine(String.Format("Invalid command line parameter \"{0}\". Press <enter> to exit.", args[0]));
                            System.Console.ReadLine();
                            break;
                    }
                }
                else
                {
                    System.Console.ForegroundColor = CONSOLE_COLOR_MAIN;
                    ShowMainMenu();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(Processor.Helpers.ExceptionHelper.GetExceptionMessagesRecursive(ex));
                System.Console.WriteLine(ex.StackTrace);
                System.Console.WriteLine("Press any key to exit");
                System.Console.Read();
            }
        }

        static void ShowMainMenu()
        {
            if (MainMenu == null)
            {
                MainMenu = new CommandMenu();
                MainMenu.Title = "Main Menu";
                MainMenu.AddCommand("Production Support menu", () => ShowProdSupportMenu());
                MainMenu.AddCommand("Testing menu", () => ShowTestingMenu());
                MainMenu.AddCommand("AVM Revision Check menu", () => ShowAvmRevisionCheckMenu());
                MainMenu.AddCommand("Exit", () => Exit());
            }
            //Todo: Reorder these options to they make sense.
            MainMenu.Show();
        }

        static void WriteHeading(string heading)
        {
            CommandMenu.ShowHeader();
            System.Console.WriteLine("");
            System.Console.WriteLine(heading);
            System.Console.WriteLine("---------------------------------\n");
        }

        private static void Done()
        {
            System.Console.WriteLine("");
            Pause(true);
        }

        private static void Pause(bool clearOnContinue)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Press any key to continue...");

            System.Console.ReadLine();
            if (clearOnContinue)
            {
                System.Console.Clear();
            }
        }

        private static void Exit()
        {
            System.Environment.Exit(0);
        }
    }
}
