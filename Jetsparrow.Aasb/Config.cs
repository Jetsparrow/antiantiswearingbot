namespace Jetsparrow.Aasb;

public class ServiceSettings
{
    public string Urls { get; set; }
}

public class UnbleeperSettings
{
    public string BleepedSwearsRegex { get; set; }
    public int MinAmbiguousWordLength { get; set; }
    public int MinWordLength { get; set; }
}

public class SearchDictionarySettings
{
    public string DictionaryPath { get; set; }
}

public class TelegramSettings
{
    public string ApiKey { get; set; }
    public bool UseProxy { get; set; }
    public string Url { get; set; }
    public int Port { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}
