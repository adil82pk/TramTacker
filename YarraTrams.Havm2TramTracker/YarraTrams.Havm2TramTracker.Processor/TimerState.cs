using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTrams.Havm2TramTracker.Processor.Helpers;

namespace YarraTrams.Havm2TramTracker.Processor
{
    class TimerState
    {
        public System.Threading.Timer TimerReference;
        public bool TimerCanceled;
        public Enums.Processes process;
    }
}
