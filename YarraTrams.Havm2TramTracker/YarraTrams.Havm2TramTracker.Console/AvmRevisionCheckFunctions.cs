using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Models;
using YarraTrams.Havm2TramTracker.Processor.Services;

namespace YarraTrams.Havm2TramTracker.Console
{
    public partial class Program
    {
        static void ShowAvmRevisionCheckMenu()
        {
            if (AvmRevisionMenu == null)
            {
                AvmRevisionMenu = new CommandMenu(MainMenu);
                AvmRevisionMenu.Title = "AVM Revision Check Menu";
                AvmRevisionMenu.AddCommand("Pull down file from AVM", () => PullDownFileFromAvm());
                AvmRevisionMenu.AddCommand("Pull down timetable(s) from HAVM2", () => PullDownTimetableFromHavm2());
                AvmRevisionMenu.AddCommand("Check tomorrows AVM timetable revision", () => CheckTomorrowsAvmTimetableRevision());
            }

            AvmRevisionMenu.Show();
        }

        private static void PullDownFileFromAvm()
        {
            string message = "";
            AvmTimetableService service = new AvmTimetableService();
            string localFilePath = Path.Combine(service.AvmLogFileArchivePath, String.Format("AvmLogFile{0}_FromConsole.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));

            var clock = new Stopwatch();

            clock.Start();
            service.DownloadFile(localFilePath);
            clock.Stop();


            message = message + string.Format("Pulling the file down took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine(String.Format("File can be found at {0}", localFilePath));

            System.Console.Write("Do you want to print the file contents here? (Y/N)");
            string input = System.Console.ReadLine();
            if (input.ToUpper() == "Y")
            {
                System.Console.WriteLine(File.ReadAllText(localFilePath));
            }

            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static void PullDownTimetableFromHavm2()
        {
            DateTime startDate = GetDateFromUser("Enter a start date (yyyy-mm-dd format), blank for default (tomorrow):") ?? DateTime.Now.AddDays(1).Date;
            DateTime endDate = GetDateFromUser("Enter a date (yyyy-mm-dd format, timetable data will start from the day entered), blank for default (same as start):") ?? startDate;

            string message = "";
            Havm2TimetableService service = new Havm2TimetableService();

            var clock = new Stopwatch();

            clock.Start();
            List<HavmTimetable> timetables = service.GetTimetables(startDate, endDate);
            clock.Stop();


            message = message + string.Format("Pulling the timetable(s) down took {0}.", clock.Elapsed);
            message = message + string.Format("{0}Using dates {1:yyyy-MM-dd} to {2:yyyy-MM-dd}.", Environment.NewLine, startDate, endDate);

            System.Console.WriteLine(message);

            PrintTimetablesToConsole(timetables);

            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static void CheckTomorrowsAvmTimetableRevision()
        {
            string message = "";
            var clock = new Stopwatch();

            clock.Start();
            Havm2TramTracker.Processor.Processor.CheckAvmTimetableRevision();
            clock.Stop();

            message = message + string.Format("Checking the AVM timetable revision took {0}.", clock.Elapsed);

            System.Console.WriteLine(message);
            System.Console.WriteLine("Complete, press <enter> to continue.");
            System.Console.ReadLine();
        }

        private static void PrintTimetablesToConsole(List<HavmTimetable> timetables)
        {
            const int maxTimetablesToPrint = 3;
            System.Console.WriteLine("{0:d} timetable(s)", timetables.Count);
            int timetableCounter = 1;
            foreach (var timetable in timetables)
            {
                System.Console.WriteLine(timetable.ToString());
                System.Console.WriteLine(new String('-', 30));

                timetableCounter++;
                if (timetableCounter > maxTimetablesToPrint && timetables.Count > maxTimetablesToPrint)
                {
                    System.Console.WriteLine("## Only printed the first {0} of {1} timetables. ##", maxTimetablesToPrint, timetables.Count);
                    break;
                }
            }
        }
    }
}
