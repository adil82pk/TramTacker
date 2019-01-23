using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using YarraTrams.Havm2TramTracker.Models;

namespace YarraTrams.Havm2TramTracker.Console
{
    public partial class Program
    {
        private static void UploadJsonFileToMemoryAndPrintToConsole()
        {
            if (UploadJsonFileToMemory(out List<HavmTrip> trips))
            {
                PrintTripsToConsole(trips);

                System.Console.WriteLine("Complete, press <enter> to continue.");
                System.Console.ReadLine();
            }
        }

        private static void UploadJsonFileToMemoryAndSaveToT_Temp_Trips()
        {
            if (UploadJsonFileToMemory(out List<HavmTrip> trips))
            {
                Processor.Processor.SaveTripsToT_Temp_Trips(trips);
                
                System.Console.WriteLine("Complete, press <enter> to continue.");
                System.Console.ReadLine();
            }
        }

        private static void CallHavm2ApiAndPrintToConsole()
        {
            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            var jsonstring = Processor.Processor.GetDataFromHavm2();
            clock.Stop();

            message = message + $"Getting data from HAVM2 took {clock.Elapsed}.";

            clock.Reset();
            clock.Start();
            var trips = Processor.Processor.CopyJsonToTrips(jsonstring);
            clock.Stop();


            message = message + $"\nPutting data in memory took {clock.Elapsed}.";

            clock.Reset();
            clock.Start();
            PrintTripsToConsole(trips);
            clock.Stop();

            message = message + $"\nPrinting to the screen took {clock.Elapsed}.";

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
            var jsonstring = Processor.Processor.GetDataFromHavm2();
            clock.Stop();

            message = message + $"Getting data from HAVM2 took {clock.Elapsed}.";

            clock.Reset();
            clock.Start();
            var trips = Processor.Processor.CopyJsonToTrips(jsonstring);
            clock.Stop();


            message = message + $"\nPutting data in memory took {clock.Elapsed}.";

            clock.Reset();
            clock.Start();
            Processor.Processor.SaveTripsToT_Temp_Trips(trips);
            clock.Stop();

            message = message + $"\nSaving to T_Temp_Trips took {clock.Elapsed}.";

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
                System.Console.WriteLine("Trip HastusTripId: {0}", trip.HastusTripId);
                System.Console.WriteLine("     Block: {0}", trip.Block);
                System.Console.WriteLine("     Direction: {0}", trip.Direction);
                System.Console.WriteLine("     DisplayCode: {0}", trip.DisplayCode);
                System.Console.WriteLine("     DistanceMetres: {0:d}", trip.DistanceMetres);
                System.Console.WriteLine("     NextDisplayCode: {0}", trip.NextDisplayCode);
                System.Console.WriteLine("     StartTime: {0:c}", trip.StartTime);
                System.Console.WriteLine("     StartTimepoint: {0}", trip.StartTimepoint);
                System.Console.WriteLine("     EndTime: {0:c}", trip.EndTime);
                System.Console.WriteLine("     EndTimepoint: {0}", trip.EndTimepoint);
                System.Console.WriteLine("     VehicleType: {0}", trip.VehicleType);
                System.Console.WriteLine("     Stops: {0:d}", trip.Stops.Count);
                int stopNum = 1;
                foreach (var stop in trip.Stops)
                {
                    System.Console.WriteLine("        Stop {0:d}: {1} arriving at {2:c}", stopNum, stop.HastusStopId, stop.PassingTime);
                    stopNum++;
                }
                tripCounter++;
                if (tripCounter >= maxTripsToPrint)
                {
                    System.Console.WriteLine("## Only printed the first {0} of {1} trips. ##",maxTripsToPrint,trips.Count);
                    break;
                }
            }
        }
    }
}
