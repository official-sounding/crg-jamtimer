using System;
using System.Collections.Generic;
using System.Text;

namespace crg_jamtimer.Handlers
{
    public interface IEventHandler
    {
        void OnEvent(InGameEvent ev);
    }
}
