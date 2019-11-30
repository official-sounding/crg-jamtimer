using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Websocket.Client;

namespace crc_jamtimer
{
    public class CRGMessageProcessor
    {
        private string RegistrationStrings { get => @$"{{
                    ""action"": ""Register"",
                    ""paths"": [
                        ""{IN_JAM}"",
                        ""{LINEUP_DIRECTION}"",
                        ""{LINEUP_UP_TIME}"",
                        ""{LINEUP_DOWN_TIME}"",
                        ""{TIMEOUT_RUNNING}""
                ]}}"; 
        }

        public string PingString { get => "{ \"action\": \"Ping\" }"; }

        private bool initialized = false;
        private bool inJam = false;
        private bool inTimeout = false;
        private bool lineupInverted = true;

        public async Task Initialize(WebsocketClient client)
        {
            initialized = false;
            await client.Send(RegistrationStrings);
        }

        public void Process(ResponseMessage msg)
        {
            var obj = JObject.Parse(msg.Text);
            if (obj.ContainsKey("state"))
            {
                var state = obj["state"].ToObject<Dictionary<string, object>>();

                if (state.ContainsKey(TIMEOUT_RUNNING))
                {
                    bool timeoutStatus = state[TIMEOUT_RUNNING] as bool? ?? false;
                    if (inTimeout != timeoutStatus)
                    {
                        inTimeout = timeoutStatus;
                        if (!inTimeout)
                        {
                            TriggerEvent(InGameEvent.EndOfTimeout);
                        }
                        else if (!inJam)
                        {
                            TriggerEvent(InGameEvent.TimeoutDuringLineup);
                        }

                    }
                }

                if (state.ContainsKey(IN_JAM))
                {
                    bool jamStatus = state[IN_JAM] as bool? ?? false;

                    if (jamStatus != inJam)
                    {
                        inJam = jamStatus;
                        if (jamStatus)
                        {
                            TriggerEvent(InGameEvent.JamStarted);
                        }
                        else
                        {
                            TriggerEvent(InGameEvent.JamEnded);
                        }
                    }
                }

                if (state.ContainsKey(LINEUP_DIRECTION))
                {
                    lineupInverted = state[LINEUP_DIRECTION] as bool? ?? false;
                }

                var lineupTime = lineupInverted ?  LINEUP_DOWN_TIME : LINEUP_UP_TIME;
                if (state.ContainsKey(lineupTime))
                {
                    var time = state[lineupTime] as long? ?? 0;
                    if (time == 25000)
                    {
                        TriggerEvent(InGameEvent.FiveSeconds);
                    }
                }

                if (!initialized)
                {
                    initialized = true;
                    Console.WriteLine("Initialized State");
                }
            }
        }

        private void TriggerEvent(InGameEvent ev) {
            if(initialized)
            {
                switch(ev)
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

        private static string IN_JAM = "ScoreBoard.InJam";
        private static string LINEUP_DIRECTION = "ScoreBoard.Clock(Lineup).Direction";
        private static string LINEUP_UP_TIME = "ScoreBoard.Clock(Lineup).Time";
        private static string LINEUP_DOWN_TIME = "ScoreBoard.Clock(Lineup).InvertedTime";
        private static string TIMEOUT_RUNNING = "ScoreBoard.Clock(Timeout).Running";
    }


    public enum InGameEvent
    {
        FiveSeconds,
        JamStarted,
        JamEnded,
        TimeoutDuringJam,
        TimeoutDuringLineup,
        EndOfTimeout
    }
}
