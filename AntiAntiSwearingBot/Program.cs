using System.Threading;
using AntiAntiSwearingBot;

static void Log(string m) => Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}|{m}");

Log("AntiAntiSwearBot starting....");

var cfg = Config.Load<Config>("aasb.cfg.json", "aasb.cfg.secret.json");
var dict = new SearchDictionary(cfg);
Log($"{dict.Count} words loaded.");
var bot = new AntiAntiSwearingBot.AntiAntiSwearingBot(cfg, dict);
bot.Init().Wait();
Log($"Connected to Telegram as @{bot.Me.Username}");
Log("AntiAntiSwearBot started! Press Ctrl-C to exit.");

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
dict.Save();

