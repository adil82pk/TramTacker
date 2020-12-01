using FluentFTP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor.Services
{
    public class AvmTimetableService
    {
        public readonly string AvmLogFileFtpServerAddress;
        public readonly string AvmLogFileFtpServerUsername;
        public readonly string AvmLogFileFtpServerPassword;
        public readonly string AvmLogFileFtpDirectoryPath;
        public readonly string AvmLogFileName;
        public readonly string AvmLogFileArchivePath;
        public readonly int AvmLogFileArchiveRetentionPeriodInDays;

        private readonly int[] ValidLineLengths = { 2, 8, 9 }; // The file has variable line lengths - 9 for the currently loaded timetable, 2 when a future day has no timetable and 8 when it does.
        private const int LineContainingTomorrowsTimetable = 3; // Tomorrow's timetable is on line 3.
        private const int FieldIndexOfNa = 1; // When there is no timetable loaded the 2nd field along (1 when zero-based) will contain the text NA.
        private const int FieldIndexOfExportTimestamp = 5; // The export timestamp is the 6th field along (5 when zero-based).

        private const string EmptyFileErrorMessage = "AVM file is empty.";
        private const string TruncatedFileErrorMessage = "AVM file appears to be truncated.";
        private const string TomorrowTimestampErrorMessage = "Expecting the second field on the third line of the file to be a timestamp";
        private const string GeneralFileContentErrorMessage = "Unexpected format inside AVM file.";

        public AvmTimetableService()
        {
            AvmLogFileFtpServerAddress = Properties.Settings.Default.AvmLogFileFtpServerAddress;
            AvmLogFileFtpServerUsername = Properties.Settings.Default.AvmLogFileFtpServerUsername;
            AvmLogFileFtpServerPassword = Properties.Settings.Default.AvmLogFileFtpServerPassword;
            AvmLogFileFtpDirectoryPath = Properties.Settings.Default.AvmLogFileFtpDirectoryPath;
            AvmLogFileName = Properties.Settings.Default.AvmLogFileName;
            AvmLogFileArchivePath = Properties.Settings.Default.AvmLogFileArchivePath;
            AvmLogFileArchiveRetentionPeriodInDays = Properties.Settings.Default.AvmLogFileArchiveRetentionPeriodInDays;
        }

        /// <summary>
        /// Connects to AVM via FTP and checks which timetable revision is due to run tomorrow.
        /// </summary>
        /// <returns>The export timestamp of the revision, as an integer.</returns>
        public int GetTomorrowsAvmTimetableRevision()
        {
            CleanupOldArchives();

            string localFilePath = Path.Combine(AvmLogFileArchivePath, String.Format("AvmLogFile{0}.txt", DateTime.Now.ToString("yyyyMMddHHmmss")));

            DownloadFile(localFilePath);

            return ExtractTomorrowsAvmTimetableRevisionFromFileContent(File.ReadAllText(localFilePath));
        }

        /// <summary>
        /// Connects to AVM via FTP, download the schedule summary file and saves it to the passed-in location.
        /// </summary>
        /// <param name="localFilePath">The location that the downloaded file should be saved to</param>
        public void DownloadFile(string localFilePath)
        {
            using (var ftp = new FtpClient(AvmLogFileFtpServerAddress, AvmLogFileFtpServerUsername, AvmLogFileFtpServerPassword) { RetryAttempts = 3 })
            {
                try
                {
                    ftp.Connect();
                }
                catch (Exception ex)
                {
                    LogWriter.Instance.Log(EventLogCodes.CANNOT_CONNECT_TO_AVM_FTP, String.Format("Cannot connect to AVM FTP.\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
                    throw;
                }

                try
                {
                    // download a file and ensure the local directory is created, verify the file after download
                    ftp.DownloadFile(localFilePath, AvmLogFileFtpDirectoryPath + AvmLogFileName, FtpLocalExists.Overwrite, FtpVerify.Retry);
                }
                catch (Exception ex)
                {
                    LogWriter.Instance.Log(EventLogCodes.CANNOT_FIND_FILE_ON_AVM_ENDPOINT, String.Format("Cannot download file from AVM FTP.\n\nMessage: {0}\n\nStacktrace:{1}", ex.Message, ex.StackTrace));
                    throw;
                }
            }
        }

        public int ExtractTomorrowsAvmTimetableRevisionFromFileContent(string content)
        {
            // If the file is empty we throw an exception.
            if (String.IsNullOrEmpty(content))
            {
                throw new FormatException(EmptyFileErrorMessage);
            }

            int timestamp = -1;
            using (StringReader sr = new StringReader(content))
            {
                string line;

                int lineNumber = 1;

                while ((line = sr.ReadLine()) != null)
                {
                    string[] lineData = line.Split(new char[] { ',' });

                    // If the file appears to be truncated then we create an event but we continue (it's likely we have enough data to read tomorrow's timestamp).
                    if (!ValidLineLengths.Contains(lineData.Length))
                    {
                        LogWriter.Instance.Log(EventLogCodes.TRUNCATED_FILE_ON_AVM_ENDPOINT, String.Format("{0}\nLine {1}:{2}", TruncatedFileErrorMessage, lineNumber, line));
                    }

                    // The timstamp for tomorrow's file is on line 3.
                    if (lineNumber == LineContainingTomorrowsTimetable)
                    {
                        // If there is no timetable loaded for tomorrow then we set the timestamp to 0.
                        if (lineData[FieldIndexOfNa] == "NA")
                        {
                            timestamp = 0;
                        }
                        else
                        {
                            // If the timestamp is not an integer then we create a specific event and abort (throw an exception).
                            if (!int.TryParse(lineData[FieldIndexOfExportTimestamp], out timestamp))
                            {
                                LogWriter.Instance.Log(EventLogCodes.UNEXPECTED_FORMAT_INSIDE_AVM_FILE, TomorrowTimestampErrorMessage);
                                throw new FormatException(TomorrowTimestampErrorMessage);
                            }
                        }
                    }

                    lineNumber++;
                }
            }

            // If the timestamp never got set then we log a specific error then abort (throw an exception).
            if (timestamp < 0)
            {
                LogWriter.Instance.Log(EventLogCodes.UNEXPECTED_FORMAT_INSIDE_AVM_FILE, GeneralFileContentErrorMessage);
                throw new FormatException(GeneralFileContentErrorMessage);
            }

            return timestamp;
        }

        /// <summary>
        /// Deletes all files in the archive that were created prior to the configured retention time.
        /// </summary>
        public void CleanupOldArchives()
        {
            string[] files = Directory.GetFiles(AvmLogFileArchivePath);

            foreach (string file in files)
            {
                FileInfo fi = new FileInfo(file);
                if (fi.CreationTime < DateTime.Now.AddMilliseconds(-AvmLogFileArchiveRetentionPeriodInDays))
                {
                    fi.Delete();
                }
            }
        }
    }
}
