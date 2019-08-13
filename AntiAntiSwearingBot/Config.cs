namespace AntiAntiSwearingBot
{
    public class Config : ConfigBase
    {
        public string ApiKey { get; private set; }

        public string BleepedSwearsRegex { get; private set; }

        public struct ProxySettings
        {
            public string Url { get; private set; }
            public int Port { get; private set; }
            public string Login { get; private set; }
            public string Password { get; private set; }
        }

        public ProxySettings Proxy { get; private set; }

        public struct SearchDictionarySettings
        {
            public string DictionaryPath { get; private set; }

            public double LearnNudgeFactor { get; private set; }
            public double LearnInitialRating { get; private set; }
            public int MinUnlearnNudge { get; private set; }
            public double UnlearnNudgeFactor { get; private set; }
        }

        public SearchDictionarySettings SearchDictionary { get; private set; }
    }
}

