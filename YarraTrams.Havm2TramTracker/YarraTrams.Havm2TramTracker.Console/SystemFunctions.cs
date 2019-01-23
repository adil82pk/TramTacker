using System.Collections.Generic;
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
                PrintTripToConsole(trips);

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

        private static void PrintTripToConsole(List<Models.HavmTrip> trips)
        {
            System.Console.WriteLine("{0:d} trips", trips.Count);
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
            }
        }
    }
}
