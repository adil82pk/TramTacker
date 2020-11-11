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

        public AvmTimetableService()
        {
            AvmLogFileFtpServerAddress = Properties.Settings.Default.AvmLogFileFtpServerAddress;
            AvmLogFileFtpServerUsername = Properties.Settings.Default.AvmLogFileFtpServerUsername;
            AvmLogFileFtpServerPassword = Properties.Settings.Default.AvmLogFileFtpServerPassword;
            AvmLogFileFtpDirectoryPath = Properties.Settings.Default.AvmLogFileFtpDirectoryPath;
            AvmLogFileName = Properties.Settings.Default.AvmLogFileName;
            AvmLogFileArchivePath = Properties.Settings.Default.AvmLogFileArchivePath;
        }

        /// <summary>
        /// Connects to AVM via FTP and checks which timetable revision is due to run tomorrow.
        /// </summary>
        /// <returns>The export timestamp of the revision, as an integer.</returns>
        public int GetTomorrowsAvmTimetableRevision()
        {
            string localFilePath = Path.Combine(AvmLogFileArchivePath, $"AvmLogFile{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");

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
            return 0;
        }
    }
}
