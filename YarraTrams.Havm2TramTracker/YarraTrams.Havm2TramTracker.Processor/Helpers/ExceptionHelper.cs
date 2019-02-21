using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Processor.Helpers
{
    class ExceptionHelper
    {
        public static string GetExceptionMessagesRecursive(Exception e, string message = "")
        {
            message = String.Format("{0}{1}{2}", message, Environment.NewLine, e.Message);

            if (e.InnerException != null)
            {
                message = ExceptionHelper.GetExceptionMessagesRecursive(e.InnerException, message);
            }

            return message;
        }
    }
}
