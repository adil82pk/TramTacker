using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTrams.Havm2TramTracker.Console
{
    public class Command
    {
        public string Shortcut { get; set; }
        public string Description { get; set; }
        public Action ActionMethod { get; set; }

        public Command(string shortcut, string description, Action actionMethod)
        {
            this.Shortcut = shortcut;
            this.Description = description;
            this.ActionMethod = actionMethod;
        }

        public void Execute()
        {
            this.ActionMethod();
        }
    }
}
