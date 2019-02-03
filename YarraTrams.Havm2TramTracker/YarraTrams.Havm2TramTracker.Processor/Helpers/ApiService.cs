using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    public class ApiService
    {
        public static string GetDataFromHavm2()
        {
            var result = Task.Run(() => {
                return ApiHttpClient.GetDataFromHavm2();
            }).Result;

            return result;
        }
    }
}
