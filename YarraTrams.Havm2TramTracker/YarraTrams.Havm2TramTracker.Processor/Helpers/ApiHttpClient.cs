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

        public static string GetDataFromHavm2()
        {
            string uri = Properties.Settings.Default.Havm2TramTrackerAPI;

            LogWriter.Instance.Log(EventLogCodes.PRE_CALL_TO_HAVM, String.Format("About to call HAVM2 service - {0}", uri));

            string response = System.Text.Encoding.Default.GetString(httpClient.DownloadData(uri));

            LogWriter.Instance.Log(EventLogCodes.POST_CALL_TO_HAVM, String.Format("Call to HAVM2 successful - {0} bytes returned.", response.Length));

            return response;
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
