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
        public static readonly HttpClient httpClient;

        static ApiHttpClient()
        {
            int timeoutInSeconds = Properties.Settings.Default.Havm2TramTrackerTimeoutSeconds; //Todo: make sure this picks up the latest config when running as a windows service.
            httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(timeoutInSeconds);
        }

        public static string GetDataFromHavm2()
        {
            string uri = Properties.Settings.Default.Havm2TramTrackerAPI; //Todo: make sure this picks up the latest config when running as a windows service.

            //Todo: Log here

            //Todo: Check code in:
            //https://bitbucket.org/ytavmis/attributiondataproviderservice/src/master/YarraTrams.ADPS/YarraTrams.ADPS.Services/OdmApiHttpClient.cs
            //https://bitbucket.org/ytavmis/attributiondataproviderservice/src/a50e66391b09e57ba7c71fa43fa0cfb3299fb64f/YarraTrams.ADPS/YarraTrams.ADPS.Services/OdmApiIntegrationService.cs?at=master#OdmApiIntegrationService.cs-180

            var response = httpClient.GetStringAsync(uri).Result;

            return response;
        }
    }
}
