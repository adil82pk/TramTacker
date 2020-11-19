using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using YarraTrams.Havm2TramTracker.Logger;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    public static class ApiHttpClient
    {
        public static readonly WebClient httpClient;

        static ApiHttpClient()
        {
            httpClient = new WebClientWithCustomTimeout();
            httpClient.UseDefaultCredentials = true;
        }

        public static string GetDataFromHavm2(DateTime startDate, DateTime endDate)
        {
            string uri = Properties.Settings.Default.Havm2TramTrackerAPI;

            uri += string.Format("?basedate={0:yyyy-MM-dd}&endDate={1:yyyy-MM-dd}", startDate, endDate);

            LogWriter.Instance.Log(EventLogCodes.PRE_CALL_TO_HAVM, String.Format("About to call HAVM2 service - {0}", uri));

            string response = System.Text.Encoding.Default.GetString(httpClient.DownloadData(uri));

            LogWriter.Instance.Log(EventLogCodes.POST_CALL_TO_HAVM, String.Format("Call to HAVM2 successful - {0} bytes returned.", response.Length));

            return response;
        }

        public static string GetTimetablesFromHavm2(DateTime startDate, DateTime endDate)
        {
            string uri = Properties.Settings.Default.Havm2TimetableAPI;

            uri += string.Format("?fromDate={0:yyyy-MM-dd}&toDate={1:yyyy-MM-dd}", startDate, endDate);

            try
            {
                return System.Text.Encoding.Default.GetString(httpClient.DownloadData(uri));
            }
            catch (WebException ex)
            {
                string message = "Error returned when trying to pull timetables from HAVM2 for the AVM timetable revision check.";

                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse response = ex.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.NotFound)
                    {
                        // If we recieved a 404 then we write a specific message.
                        message = message + " HAVM2 returned a 404, which usually means there are no timetables for this date range.";
                    }
                }
                
                LogWriter.Instance.Log(EventLogCodes.ERROR_WHEN_CONNECTING_TO_HAVM2, message);
                throw;
            }
        }
    }

    internal class WebClientWithCustomTimeout : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            int timeoutInSeconds = Properties.Settings.Default.Havm2TramTrackerAPITimeoutSeconds;
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = timeoutInSeconds * 1000;
            return w;
        }
    }
}
