using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    public static class LogfileWriter
    {
        /// <summary>
        /// Writes string data to a file, pre-pending the filename with the date.
        /// </summary>
        /// <param name="postFix">The filename is prefixed with the current date then appends this postFix</param>
        /// <param name="data">The string data that is written to the file</param>
        /// <param name="path">The directory of the file</param>
        public static void writeToFile(string postFix, string data, string path)
        {
            string logFilename = String.Format("{0}-{1}.txt", DateTime.Now.ToString("yyyy_MM_dd_HH_mm"), postFix);
            writeToFile(path + @"\" + logFilename, data);
        }

        /// <summary>
        /// Writes string data to a file.
        /// </summary>
        /// <param name="logfileNameAndPath">The name of the file, including full directory path</param>
        /// <param name="data">The string data that is written to the file</param>
        public static void writeToFile(string logfileNameAndPath, string data)
        {
            try
            {
                // Check that our log folder is not at capacity
                long logDirSize = LogfileWriter.DirSize(new DirectoryInfo(Properties.Settings.Default.LogFilePath));

                if (logDirSize >= Properties.Settings.Default.LogFilePathMaxSizeInBytes)
                {
                    LogWriter.Instance.Log(
                        EventLogCodes.LOG_FOLDER_REACHED_CAPACITY,
                        String.Format("Log folder ({0}) reached capacity of {1} bytes.", Properties.Settings.Default.LogFilePath, Properties.Settings.Default.LogFilePathMaxSizeInBytes));

                    return;
                }
                else if (logDirSize >= Properties.Settings.Default.LogFilePathWarnSizeInBytesExceedsInBytes)
                {
                    LogWriter.Instance.Log(
                        EventLogCodes.LOG_FOLDER_APPROACHING_CAPACITY,
                        String.Format("Log folder ({0}) approaching capacity of {1} bytes.", Properties.Settings.Default.LogFilePath, Properties.Settings.Default.LogFilePathMaxSizeInBytes));
                }

                using (StreamWriter fileWriter = new StreamWriter(logfileNameAndPath, true))
                {
                    fileWriter.Write(data);
                }
            }
            catch (Exception e)
            {
                LogWriter.Instance.LogWithoutDelay(EventLogCodes.LOG_FILE_WRITE_ERROR, String.Format("Open file writer failed\n\nMessage: {0}\n\nStacktrace:{1}", e.Message, e.StackTrace));
            }
        }

        /// <summary>
        /// Get/generate the file name and path for a log file.
        /// </summary>
        /// <param name="namePrefixFragment"></param>
        /// <returns></returns>
        public static string GetFilePathAndName(string namePrefixFragment)
        {
            return GetFilePathAndName(Properties.Settings.Default.LogFilePath, namePrefixFragment);
        }

        /// <summary>
        /// Get/generate the file name and path for a log file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="namePrefixFragment"></param>
        /// <returns></returns>
        public static string GetFilePathAndName(string path, string namePrefixFragment)
        {
            return Path.Combine(path, string.Format("{0}_{1}.log", namePrefixFragment, DateTime.Now.ToString("yyyy_MM_dd_HH_mm")));
        }

        /// <summary>
        /// Returns the total directory size, in bytes.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;

            // Add file sizes.
            FileInfo[] fis = d.GetFiles();

            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }

            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();

            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }

            return size;
        }

        /// <summary>
        /// Formats the string with a header and footer.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static string ToLogString(this string str, string description)
        {
            return DateTime.Now.ToString("yyyy-MM-ddThHH:mm:ss.fff ") + description + "\n" + str + "\n" + LogfileWriter.GetLogFooter();
        }

        /// <summary>
        /// Get the text to write between two log entries.
        /// </summary>
        private static string GetLogFooter()
        {
            const char chr = '#';
            const Int16 length = 120;

            return "\n" + new String(chr, length) + "\n" + new String(chr, length) + "\n" + new String(chr, length) + "\n\n\n";
        }
    }
}
