using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    class LogfileWriter
    {
        /// <summary>
        /// Writes string data to a file, pre-pending the filename with the date.
        /// </summary>
        /// <param name="postFix">The filename is prefixed with the current date then appends this postFix</param>
        /// <param name="data">The string data that is written to the file</param>
        /// <param name="path">The directory of the file</param>
        public static void writeToFile(string postFix, string data, string path)
        {
            string logFilename = String.Format("{0}-{1}.txt", DateTime.Now.ToString("yyyy-MM-dd"), postFix);
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
    }
}
