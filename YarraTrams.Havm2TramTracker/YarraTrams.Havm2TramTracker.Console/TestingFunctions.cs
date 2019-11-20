using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using YarraTrams.Havm2TramTracker.Models;

namespace YarraTrams.Havm2TramTracker.Console
{
    public partial class Program
    {
        static void ShowTestingMenu()
        {
            if (TestingMenu == null)
            {
                TestingMenu = new CommandMenu(MainMenu);
                TestingMenu.Title = "Testing Menu";
                TestingMenu.AddCommand("Upload JSON file to memory and print to console", () => UploadJsonFileToMemoryAndPrintToConsole());
                TestingMenu.AddCommand("Upload JSON file to T_Temp_Trips database table", () => UploadJsonFileToMemoryAndSaveToT_Temp_Trips());
                TestingMenu.AddCommand("Upload JSON file to T_Temp_Schedules database table", () => UploadJsonFileToMemoryAndSaveToT_Temp_Schedules());
                TestingMenu.AddCommand("Call HAVM2 API and print to console", () => CallHavm2ApiAndPrintToConsole());
                TestingMenu.AddCommand("Call HAVM2 API and save to T_Temp_Trips database table", () => CallHavm2ApiAndSaveToT_Temp_Trips());
                TestingMenu.AddCommand("Call HAVM2 API and save to T_Temp_Schedules database table", () => CallHavm2ApiAndSaveToT_Temp_Schedules());
                TestingMenu.AddCommand("Call HAVM2 API and save to T_Temp_SchedulesMaster database table", () => CallHavm2ApiAndSaveToT_Temp_SchedulesMaster());
                TestingMenu.AddCommand("Call HAVM2 API and save to T_Temp_SchedulesDetails database table", () => CallHavm2ApiAndSaveToT_Temp_SchedulesDetails());
                TestingMenu.AddCommand("Compare Existing and New data", () => CompareData());
            }

            TestingMenu.Show();
        }

        private static void UploadJsonFileToMemoryAndPrintToConsole()
        {
            List<HavmTrip> trips;
            if (UploadJsonFileToMemory(out trips))
            {
                PrintTripsToConsole(trips);

                System.Console.WriteLine("Complete, press <enter> to continue.");
                System.Console.ReadLine();
            }
        }

        private static void UploadJsonFileToMemoryAndSaveToT_Temp_Trips()
        {
            List<HavmTrip> trips;
            if (UploadJsonFileToMemory(out trips))
            {
                Processor.Processor.SaveToTrips(trips);

                System.Console.WriteLine("Complete, press <enter> to continue.");
                System.Console.ReadLine();
            }
        }

        private static void UploadJsonFileToMemoryAndSaveToT_Temp_Schedules()
        {
            List<HavmTrip> trips;
            if (UploadJsonFileToMemory(out trips))
            {
                Processor.Processor.SaveToSchedules(trips);

                System.Console.WriteLine("Complete, press <enter> to continue.");
                System.Console.ReadLine();
            }
        }

        private static void CallHavm2ApiAndPrintToConsole()
        {
            DateTime? baseDate = GetDateFromUser("Enter a date (data will start from the day following), blank for default:");

            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            var jsonstring = Processor.Helpers.ApiService.GetDataFromHavm2(baseDate);
            clock.Stop();

            message = message + string.Format("Getting data from HAVM2 took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            var trips = Processor.Processor.CopyJsonToTrips(jsonstring);
            clock.Stop();
            
            message = message + string.Format("\nPutting data in memory took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            PrintTripsToConsole(trips);
            clock.Stop();

            message = message + string.Format("\nPrinting to the screen took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();

            //Figures for all trips/stops on first test:
            //Getting data from HAVM2 took 00:07:32.9649207. Too long.
            //Putting data in memory took 00:00:13.1290774. Great.
            //Printing to the screen took 00:23:12.8448444. Irrelevant.
        }

        private static void CallHavm2ApiAndSaveToT_Temp_Trips()
        {
            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            var jsonstring = Processor.Helpers.ApiService.GetDataFromHavm2(null);
            clock.Stop();

            message = message + string.Format("Getting data from HAVM2 took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            var trips = Processor.Processor.CopyJsonToTrips(jsonstring);
            clock.Stop();

            message = message + string.Format("\nPutting data in memory took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            Processor.Processor.SaveToTrips(trips);
            clock.Stop();

            message = message + string.Format("\nSaving to T_Temp_Trips took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static void CallHavm2ApiAndSaveToT_Temp_Schedules()
        {
            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            var jsonstring = Processor.Helpers.ApiService.GetDataFromHavm2(null);
            clock.Stop();

            message = message + string.Format("Getting data from HAVM2 took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            var trips = Processor.Processor.CopyJsonToTrips(jsonstring);
            clock.Stop();

            message = message + string.Format("\nPutting data in memory took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            Processor.Processor.SaveToSchedules(trips);
            clock.Stop();

            message = message + string.Format("\nSaving to T_Temp_Schedules took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static void CallHavm2ApiAndSaveToT_Temp_SchedulesMaster()
        {
            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            var jsonstring = Processor.Helpers.ApiService.GetDataFromHavm2(null);
            clock.Stop();

            message = message + string.Format("Getting data from HAVM2 took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            var trips = Processor.Processor.CopyJsonToTrips(jsonstring);
            clock.Stop();

            message = message + string.Format("\nPutting data in memory took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            Processor.Processor.SaveToSchedulesMaster(trips);
            clock.Stop();

            message = message + string.Format("\nSaving to T_Temp_SchedulesMaster took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static void CallHavm2ApiAndSaveToT_Temp_SchedulesDetails()
        {
            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            var jsonstring = Processor.Helpers.ApiService.GetDataFromHavm2(null);
            clock.Stop();

            message = message + string.Format("Getting data from HAVM2 took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            var trips = Processor.Processor.CopyJsonToTrips(jsonstring);
            clock.Stop();

            message = message + string.Format("\nPutting data in memory took {0}.", clock.Elapsed);

            clock.Reset();
            clock.Start();
            Processor.Processor.SaveToSchedulesDetails(trips);
            clock.Stop();

            message = message + string.Format("\nSaving to T_Temp_SchedulesDetails took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static void CompareData()
        {
            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            var x = new TestComparisons.Comparisons();
            x.RunComparisons();
            clock.Stop();

            message = message + string.Format("Comparing the data took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static bool UploadJsonFileToMemory(out List<HavmTrip> trips)
        {
            System.Console.WriteLine("Enter full path to a JSON file containing valid HAVM2 TramTRACKER data:");
            string filepath = System.Console.ReadLine();

            if (!File.Exists(filepath))
            {
                System.Console.WriteLine("File not found");
                Done();
                trips = new List<HavmTrip>();
                return false;
            }
            else
            {
                trips = Processor.Processor.CopyJsonToTrips(File.ReadAllText(filepath));
                return true;
            }
        }

        private static void PrintTripsToConsole(List<Models.HavmTrip> trips)
        {
            const int maxTripsToPrint = 1;
            System.Console.WriteLine("{0:d} trips", trips.Count);
            int tripCounter = 1;
            foreach (var trip in trips)
            {
                System.Console.WriteLine(trip.ToString());

                tripCounter++;
                if (tripCounter >= maxTripsToPrint)
                {
                    System.Console.WriteLine("## Only printed the first {0} of {1} trips. ##",maxTripsToPrint,trips.Count);
                    break;
                }
            }
        }

        /// <summary>
        /// Get the user to enter a date. If they enter a valid one, we return it, if they enter blank, we return null, otherwise we show an error message and return null.
        /// </summary>
        private static DateTime? GetDateFromUser(string message)
        {
            System.Console.WriteLine(message);
            string input = System.Console.ReadLine();
            DateTime inputDate;
            DateTime? baseDate = null; // Default to null if blank was entered.
            if (input != "" && DateTime.TryParse(input, out inputDate)) // If a valid date was entered, use it.
            {
                baseDate = inputDate;
            }
            else if (input != "") // If junk was entered, show an error then take the null default.
            {
                System.Console.WriteLine(string.Format("Error: Invalid date format ({0}). Date will revert to the default. Press <enter> to continue.", input));
                System.Console.ReadLine();
            }
            return baseDate;
        }
    }
}
