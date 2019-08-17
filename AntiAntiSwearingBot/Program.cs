using System;

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

        public static int Main(string[] args)
        {
            try
            {
                var cfg = Config.Load<Config>("aasb.cfg.json", "aasb.cfg.secret.json");
                var dict = new SearchDictionary(cfg);
                var bot = new AntiAntiSwearingBot(cfg, dict);
                bot.Init().Wait();
                Console.WriteLine("AntiAntiSwear started. Press any key to exit...");
                Environment.ExitCode = (int)ExitCode.ErrorRunning;
                Console.ReadKey();
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
