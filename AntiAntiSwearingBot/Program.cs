using System;
using System.Threading;

namespace AntiAntiSwearingBot
{
    public static class Program
    {
        public enum ExitCode : int
        {
            Ok = 0,
            ErrorNotStarted = 0x80,
            ErrorRunning = 0x81,
            ErrorException = 0x82,
            ErrorInvalidCommandLine = 0x100
        };

        static void Log(string m) => Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}|{m}");

        public static int Main(string[] args)
        {
            try
            {
                Log("AntiAntiSwearBot starting....");

                var cfg = Config.Load<Config>("aasb.cfg.json", "aasb.cfg.secret.json");
                var dict = new SearchDictionary(cfg);
                Log($"{dict.Count} words loaded.");
                var bot = new AntiAntiSwearingBot(cfg, dict);
                bot.Init().Wait();
                Log($"Connected to Telegram as @{bot.Me.Username}");
                Log("AntiAntiSwearBot started! Press Ctrl-C to exit.");
                Environment.ExitCode = (int)ExitCode.ErrorRunning;

                ManualResetEvent quitEvent = new ManualResetEvent(false);
                try
                {
                    Console.CancelKeyPress += (sender, eArgs) => // ctrl-c
                    {
                        eArgs.Cancel = true;
                        quitEvent.Set();
                    };
                }
                catch { }

                quitEvent.WaitOne(Timeout.Infinite);

                Console.WriteLine("Waiting for exit...");
                bot.Stop().Wait();
                dict.Save();
                return (int)ExitCode.Ok;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return (int)ExitCode.ErrorException;
            }
        }
    }
}
