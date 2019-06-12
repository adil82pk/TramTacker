using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Console
{
    public partial class Program
    {
        static void ShowProdSupportMenu()
        {
            if (ProdSupportMenu == null)
            {
                ProdSupportMenu = new CommandMenu(MainMenu);
                ProdSupportMenu.Title = "Production Support Menu";
                ProdSupportMenu.AddCommand("Copy to live tables", () => CallCopyToLive());
                ProdSupportMenu.AddCommand("Call HAVM2 API and save to all staging/temp tables", () => CallHavm2ApiAndSaveToAllTables());
            }

            ProdSupportMenu.Show();
        }

        private static void CallCopyToLive()
        {
            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            Processor.Helpers.DBHelper.CopyDataFromTempToLive();
            clock.Stop();

            message = message + string.Format("Copying data from temp to live took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static void CallHavm2ApiAndSaveToAllTables()
        {
            DateTime? baseDate = GetDateFromUser("Enter a date (data will start from the day following), blank for default:");

            string message = "";
            var clock = new Stopwatch();

            // Get schedule data from HAVM2
            clock.Start();
            string json = Processor.Helpers.ApiService.GetDataFromHavm2(baseDate);
            clock.Stop();
            message += string.Format("Getting data from HAVM2 took {0}.", clock.Elapsed);

            // Create Havm model from JSON
            clock.Reset();
            clock.Start();
            List<Models.HavmTrip> havmTrips = Processor.Processor.CopyJsonToTrips(json);
            clock.Stop();
            message += string.Format("\nPutting data in memory took  {0}.", clock.Elapsed);

            // Populate T_Temp_Trips
            clock.Reset();
            clock.Start();
            Processor.Processor.SaveToTrips(havmTrips);
            clock.Stop();
            message += string.Format("\nSaving to T_Temp_Trips took {0}.", clock.Elapsed);

            // Populate T_Temp_Schedules
            clock.Reset();
            clock.Start();
            Processor.Processor.SaveToSchedules(havmTrips);
            clock.Stop();
            message += string.Format("\nSaving to T_Temp_Schedules took {0}.", clock.Elapsed);

            // Populate T_Temp_SchedulesMaster
            clock.Reset();
            clock.Start();
            Processor.Processor.SaveToSchedulesMaster(havmTrips);
            clock.Stop();
            message += string.Format("\nSaving to T_Temp_SchedulesMaster took {0}.", clock.Elapsed);

            // Populate T_Temp_SchedulesDetails
            clock.Reset();
            clock.Start();
            Processor.Processor.SaveToSchedulesDetails(havmTrips);
            clock.Stop();
            message += string.Format("\nSaving to T_Temp_SchedulesDetails took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }
    }
}
