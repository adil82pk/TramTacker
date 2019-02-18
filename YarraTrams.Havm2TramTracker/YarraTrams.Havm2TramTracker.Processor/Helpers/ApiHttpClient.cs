using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;

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
            string uri = Properties.Settings.Default.Havm2TramTrackerAPI; //Todo: make sure this picks up the latest config when running as a windows service.

            //Todo: Log here

            string response = System.Text.Encoding.Default.GetString(httpClient.DownloadData(uri));

            return response;
        }
    }

    internal class WebClientWithCustomTimeout : WebClient
    {
        protected override WebRequest GetWebRequest(Uri uri)
        {
            int timeoutInSeconds = Properties.Settings.Default.Havm2TramTrackerTimeoutSeconds; //Todo: make sure this picks up the latest config when running as a windows service.
            WebRequest w = base.GetWebRequest(uri);
            w.Timeout = timeoutInSeconds * 1000;
            return w;
        }
    }
}
