using System;
using System.Collections.Generic;
using System.Text;

namespace crg_jamtimer.Handlers
{
    public class ConsoleHandler : IEventHandler
    {
        public void OnEvent(InGameEvent ev)
        {
            switch (ev)
            {
                case InGameEvent.FiveSeconds:
                    Console.WriteLine("FIVE SECONDS!");
                    break;
                case InGameEvent.JamStarted:
                    Console.WriteLine("Start Jam");
                    break;
                case InGameEvent.JamEnded:
                    Console.WriteLine("End Jam");
                    break;
                case InGameEvent.TimeoutDuringLineup:
                    Console.WriteLine("Timeout");
                    break;
                case InGameEvent.EndOfTimeout:
                    Console.WriteLine("End of Timeout");
                    break;
            }
        }
    }
}
