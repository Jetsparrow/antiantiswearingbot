namespace AntiAntiSwearingBot;

public class Config : ConfigBase
{
    public string ApiKey { get; private set; }
    public ProxySettings Proxy { get; private set; }
    public SearchDictionarySettings SearchDictionary { get; private set; }
    public UnbleeperSettings Unbleeper { get; private set; }
}

public struct UnbleeperSettings
{
    public string BleepedSwearsRegex { get; private set; }
    public int MinAmbiguousWordLength { get; private set; }
    public int MinWordLength { get; private set; }
}

public struct SearchDictionarySettings
{
    public string DictionaryPath { get; private set; }
}

public struct ProxySettings
{
    public string Url { get; private set; }
    public int Port { get; private set; }
    public string Login { get; private set; }
    public string Password { get; private set; }
}
