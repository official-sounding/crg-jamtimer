using System;
using System.Collections.Generic;
using System.Text;

namespace crc_jamtimer.Handlers
{
    public interface IEventHandler
    {
        void OnEvent(InGameEvent ev);
    }
}
