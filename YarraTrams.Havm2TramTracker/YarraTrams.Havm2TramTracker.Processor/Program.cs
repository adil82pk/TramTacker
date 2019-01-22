using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Processor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1() 
                //Todo: Find another time-based YT Windows service to model from.
                //Todo: Add logging project and enums, et. al.
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
