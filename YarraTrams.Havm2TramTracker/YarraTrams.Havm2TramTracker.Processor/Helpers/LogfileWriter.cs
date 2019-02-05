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
        public static void writeToFile(string postFix, string data, string path)
        {
            string logFilename = String.Format("{0}-{1}.txt", DateTime.Now.ToString("yyyy-MM-dd"), postFix);
            try
            {
                using (StreamWriter fileWriter = new StreamWriter(path + @"\" + logFilename, true))
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
