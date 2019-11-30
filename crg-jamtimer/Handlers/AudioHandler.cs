using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace crg_jamtimer.Handlers
{
    public class AudioHandler : IEventHandler
    {
        public void OnEvent(InGameEvent ev)
        {
            switch (ev)
            {
                case InGameEvent.FiveSeconds:
                    Console.WriteLine("FIVE SECONDS!");
                    break;
                case InGameEvent.JamStarted:
                    PlayFile("single-whistle.wav");
                    break;
                case InGameEvent.JamEnded:
                    PlayFile("four-whistles.wav");
                    break;
                case InGameEvent.TimeoutDuringLineup:
                    PlayFile("four-whistles.wav");
                    break;
                case InGameEvent.EndOfTimeout:
                    PlayFile("rolling-whistle.wav");
                    break;
            }
        }

        private void PlayFile(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using Stream stream = assembly.GetManifestResourceStream($"crg_jamtimer.Sounds.{filename}");
            using var audioFile = new WaveFileReader(stream);
            using var outputDevice = new WaveOutEvent();


                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            
        }
    }
}
