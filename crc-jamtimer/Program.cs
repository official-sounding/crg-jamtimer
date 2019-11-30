using crc_jamtimer.Handlers;
using Fclp;
using System;
using System.Threading;
using System.Threading.Tasks;
using Websocket.Client;

namespace crc_jamtimer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var p = BuildArgParser();

            var result = p.Parse(args);

            if (!result.HasErrors)
            {
                var argObj = p.Object;

                using var exitEvent = new ManualResetEvent(false);
                var processor = new CRGMessageProcessor(new[] { new ConsoleHandler() });

                

                using var client = new WebsocketClient(argObj.WS)
                {
                    ReconnectTimeoutMs = (int)TimeSpan.FromSeconds(30).TotalMilliseconds
                };



                client.ReconnectionHappened.Subscribe(async type => {
                    Console.WriteLine($"Reconnection happened, type: {type}");
                    await processor.Initialize(client);
                });

                client.MessageReceived.Subscribe(processor.Process);

                await client.Start();

                Console.WriteLine($"Connected to {argObj.WS}");

                await processor.Initialize(client);
                InitTimer((state) => client.Send(processor.PingString));

                exitEvent.WaitOne();
            }
        }

        private static Timer InitTimer(TimerCallback cb)
        {
            return new Timer(cb, null, 0, 10000);
        }

        private static FluentCommandLineParser<ApplicationArguments> BuildArgParser()
        {
            var p = new FluentCommandLineParser<ApplicationArguments>();

            p.Setup(arg => arg.Hostname)
                .As('h', "hostname")
                .WithDescription("Hostname the CRG Server is running on")
                .SetDefault("localhost");

            p.Setup(arg => arg.Port)
                .As('p', "port")
                .WithDescription("Port the CRG Server is running on")
                .SetDefault(8000);

            p.Setup(arg => arg.Silent)
                .As('s', "silent")
                .WithDescription("Silent-Only Mode");

            p.SetupHelp("?", "help")
                .Callback(text => Console.WriteLine(text));


            return p;
        }
    }

    public class ApplicationArguments
    {
        public string Hostname { get; set; }
        public int Port { get; set; }
        public bool Silent { get; set; }

        public Uri WS => new Uri($"ws://{Hostname}:{Port}/WS");
    }
}
