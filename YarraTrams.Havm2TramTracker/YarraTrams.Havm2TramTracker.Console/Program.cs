﻿using System;
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

        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "sidebyside":
                        var x = new SideBySideTests.Comparisons();
                        x.RunComparisons();
                        break;
                    default:
                        System.Console.WriteLine($"Invalid command line parameter \"{args[0]}\". Press <enter> to exit.");
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

        static void ShowMainMenu()
        {
            if (MainMenu == null)
            {
                MainMenu = new CommandMenu();
                MainMenu.Title = "Main Menu";
                MainMenu.AddCommand("Upload JSON file to memory and print to console", () => UploadJsonFileToMemoryAndPrintToConsole());
                MainMenu.AddCommand("Upload JSON file to T_Temp_Trips database table", () => UploadJsonFileToMemoryAndSaveToT_Temp_Trips());
                MainMenu.AddCommand("Call HAVM2 API and print to console", () => CallHavm2ApiAndPrintToConsole());
                MainMenu.AddCommand("Call HAVM2 API and save to T_Temp_Trips database table", () => CallHavm2ApiAndSaveToT_Temp_Trips());
                MainMenu.AddCommand("Call HAVM2 API and save to T_Temp_Schedules database table", () => CallHavm2ApiAndSaveToT_Temp_Schedules());
                MainMenu.AddCommand("Call HAVM2 API and save to T_Temp_SchedulesMaster database table", () => CallHavm2ApiAndSaveToT_Temp_SchedulesMaster());
                MainMenu.AddCommand("Call HAVM2 API and save to T_Temp_SchedulesDetails database table", () => CallHavm2ApiAndSaveToT_Temp_SchedulesDetails());
                MainMenu.AddCommand("Compare Existing and New data", () => CompareData());
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
